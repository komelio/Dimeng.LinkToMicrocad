using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dimeng.LinkToMicrocad.Drawing.CAD;
using Dimeng.WoodEngine.Entities;
using Dimeng.WoodEngine.Entities.Machines.Tools;
using Dimeng.WoodEngine.Entities.Machinings;
using Dimeng.WoodEngine.Math;
using Offset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.LinkToMicrocad.Drawing
{
    public class RouteDrawer
    {
        ToolFile toolfile;
        public RouteDrawer(ToolFile toolfile)
        {
            this.toolfile = toolfile;
        }

        public void Draw(Solid3d solid, Part part)
        {
            foreach (var r in part.Routings)
            {
                if (r.Points.Count > 2)
                {
                    continue;//绘制有问题，先跳过复杂的铣型，后面再来改进
                }

                Tool tool = toolfile.GetRouteToolByName(r.ToolName);
                if (tool == null)
                {
                    throw new Exception("Unknown tool name! - " + r.ToolName);
                }
                else if (tool.Diameter <= 0)
                {
                    throw new Exception("Tool diameter must above zero ! -" + r.ToolName);
                }

                using (Polyline3d plSection = getRouteSharp(tool, r))
                using (Polyline3d plSectionHalf = getRouteSharpHalf(tool, r))
                {
                    double depth = r.Points.Max(it => it.Z);

                    foreach (var s in getPathSolids(plSection, plSectionHalf, r, tool))
                    {
                        moveSolidToDepth(r, s, depth);
                        solid.BooleanOperation(BooleanOperationType.BoolSubtract, s);
                        s.Dispose();
                    }
                }

            }
        }

        private List<Solid3d> getPathSolids(Polyline3d plSection, Polyline3d plSectionHalf, Routing route, Tool tool)
        {
            List<Solid3d> solids = new List<Solid3d>();

            //step1：把截面竖起来          
            rotateEntityToZPlane(plSection, route);
            rotateEntityToZPlane(plSectionHalf, route);

            //step1.1:得到圆柱体模拟刀型
            Solid3d solidToolFrustum = new Solid3d();
            MakeToolFrustum(plSectionHalf, solidToolFrustum);

            List<Curve> curves = new List<Curve>();//保存各种分段的数据

            //step2:获得路径多段线
            using (Polyline plRoutePath = getRoutePolyline2(route, tool))
            {
                //addToDrawing(plRoutePath);
                if (route.ToolComp == ToolComp.None)
                {
                    DBObjectCollection entities = new DBObjectCollection();
                    plRoutePath.Explode(entities);

                    foreach (var entity in entities)
                    {
                        if (entity is Curve)
                        {
                            curves.Add((Curve)entity);
                        }
                    }
                }
                else
                {
                    bool isLeft = (route.ToolComp == ToolComp.Left) ? false : true;
                    curves = OffsetSegmentsCalculator.GetValue(plRoutePath, tool.Diameter / 2, isLeft);
                }
            }

            //step3:画出每段curve的solid
            foreach (Curve curve in curves)
            {
                if (double.IsNaN(curve.StartPoint.X) || double.IsNaN(curve.StartPoint.Y) ||
                    double.IsNaN(curve.EndPoint.X) || double.IsNaN(curve.EndPoint.Y))
                {
                    continue;
                }

                Entity plSectionClone = RotateEntityToRoutePathVertical(plSection, curve);

                //每一段的curve都进行扫掠
                //把curve移动到原点
                Point3d pt = curve.StartPoint;
                curve.TransformBy(Matrix3d.Displacement(Point3d.Origin - curve.StartPoint));

                //addToDrawing(curve);
                //addToDrawing(plSectionClone);
                //continue;

                //扫掠
                Solid3d curveSolid = new Solid3d();
                SweepOptionsBuilder sob = new SweepOptionsBuilder();
                sob.Align = SweepOptionsAlignOption.AlignSweepEntityToPath;
                sob.BasePoint = curve.StartPoint;

                sob.Bank = true;
                curveSolid.CreateSweptSolid(plSectionClone, curve, sob.ToSweepOptions());

                //模拟下刀点起始的圆
                using (Solid3d solidToolFrustumClone = (Solid3d)solidToolFrustum.Clone())
                {
                    curveSolid.BooleanOperation(BooleanOperationType.BoolUnite, solidToolFrustumClone);
                }
                using (Solid3d solidToolFrustumClone = (Solid3d)solidToolFrustum.Clone())
                {
                    solidToolFrustumClone.TransformBy(Matrix3d.Displacement(curve.EndPoint - Point3d.Origin));
                    curveSolid.BooleanOperation(BooleanOperationType.BoolUnite, solidToolFrustumClone);
                }

                //把扫掠的solid移动回原来的位置
                curveSolid.TransformBy(Matrix3d.Displacement(pt - Point3d.Origin));

                solids.Add(curveSolid);

                solidToolFrustum.Dispose();
                curve.Dispose();
                plSectionClone.Dispose();

                //addToDrawing(line);

                //pathSolid.BooleanOperation(BooleanOperationType.BoolUnite, line);
            }

            return solids;
        }

        private Polyline getRoutePolyline2(Routing route, Tool tool)
        {
            //获得机加工原点的转换坐标系的矩阵        
            Matrix3d matx = Matrix3d.AlignCoordinateSystem(Point3d.Origin,
                Vector3d.XAxis,
                Vector3d.YAxis,
                Vector3d.ZAxis,
                new Point3d(route.Part.MPPoint.X, route.Part.MPPoint.Y, 0),//不直接使用MPPoint，因为那个点的z值为板件表面
                route.Part.MPXAxis,
                route.Part.MPYAxis,
                Vector3d.ZAxis);//route.Part.MPZAxis);

            //获得加工路径的polyline（只有2d才支持圆弧）  
            //此时是按照机加工原点定位的，位于z=0的位置    
            Polyline plrouteline = new Polyline();
            plrouteline.Normal = Vector3d.ZAxis;

            //把数组中的每个点加到Polyline中，注意闭合和非闭不同  
            for (int i = 0; i < route.Points.Count; i++)
            {
                Point3d pto = route.Points[i];
                //pto = pto.TransformBy(matx);            
                double x = System.Math.Round(pto.X, 0);//取整数，是因为有的情况小数位会导致cad计算多段线自交时出错  
                double y = System.Math.Round(pto.Y, 0);
                Point2d pt = new Point2d(x, y);
                plrouteline.AddVertexAt(i, pt, -route.Bulges[i], 0, 0);
            }
            plrouteline.TransformBy(matx);//变换 
            return plrouteline;
        }

        private Entity RotateEntityToRoutePathVertical(Entity plSection, Curve curve)
        {
            double rotateAngle = 0;//最终的旋转角度     
            Point3d pt2nd = curve.EndPoint;

            #region 如果第一个pl是直线，就按直线的计算方法，计算整个截面的旋转角度
            double angel = System.Math.Atan2(pt2nd.Y - curve.StartPoint.Y, pt2nd.X - curve.StartPoint.X);
            if (pt2nd.X >= 0 && pt2nd.Y >= 0)
            {
                rotateAngle = angel - System.Math.PI / 2;
            }
            else if (pt2nd.X <= 0 && pt2nd.Y >= 0)
            {
                rotateAngle = angel - System.Math.PI / 2;
            }
            else if (pt2nd.X <= 0 && pt2nd.Y <= 0)
            {
                rotateAngle = System.Math.PI / 2 * 3 + angel;
            }
            else if (pt2nd.X >= 0 && pt2nd.Y <= 0)
            {
                rotateAngle = angel - System.Math.PI / 2;
            }
            #endregion

            #region 不是直线，就是曲线了，需要计算对应的切向量
            if (curve is Arc)
            {
                Arc arc = curve as Arc;

                double bulge = Math.Tan(arc.Length / arc.Radius / 4);
                //根据圆的外角关系，计算切向量和直线向量的夹角    
                double dist = System.Math.Sqrt(pt2nd.X * pt2nd.X + pt2nd.Y * pt2nd.Y);
                double h = System.Math.Abs(dist / 2 * bulge);
                double ang = System.Math.Atan2(h, dist / 2) * 2;//切线与两点直线的夹角    
                double radius = dist / 2 / System.Math.Sin(ang);
                int factor = (bulge < 0) ? 1 : -1;
                rotateAngle = rotateAngle + ang * factor;
            }
            #endregion
            //得到旋转矩阵      
            Matrix3d matrix = Matrix3d.Rotation(rotateAngle, Vector3d.ZAxis, Point3d.Origin);
            //把截面进行旋转      
            Entity cloneone = (Entity)plSection.Clone();
            cloneone.TransformBy(matrix);
            return cloneone;
        }

        private static void MakeToolFrustum(Entity plSection, Solid3d solidToolFrustum)
        {
            Entity plSect = plSection.Clone() as Entity;
            RevolveOptionsBuilder rob = new RevolveOptionsBuilder();
            rob.CloseToAxis = false;
            rob.DraftAngle = 0;
            rob.TwistAngle = 0;
            solidToolFrustum.CreateRevolvedSolid(plSect, Point3d.Origin, Vector3d.ZAxis, 2 * System.Math.PI, 0, rob.ToRevolveOptions());
        }

        private void rotateEntityToZPlane(Polyline3d plSection, Routing route)
        {
            if (route.OnTopFace)
            {
                plSection.TransformBy(Matrix3d.Rotation(System.Math.PI / 2, Vector3d.XAxis, Point3d.Origin));
            }
            else
            {
                plSection.TransformBy(Matrix3d.Rotation(-System.Math.PI / 2, Vector3d.XAxis, Point3d.Origin));
            }
        }

        private Polyline3d getRouteSharpHalf(Tool tool, Routing route)
        {
            double height = 400;//设定一个高度  
            Point3d pt1 = new Point3d(tool.Diameter / 2, 0, 0);
            Point3d pt2 = new Point3d(0, 0, 0);
            Point3d pt3 = new Point3d(0, height, 0);
            Point3d pt4 = new Point3d(tool.Diameter / 2, height, 0);
            Point3d pt5 = new Point3d(tool.Diameter / 2, 0, 0);
            Point3dCollection pts = new Point3dCollection() { pt1, pt2, pt3, pt4, pt5 };
            Polyline3d pline = new Polyline3d(Poly3dType.SimplePoly, pts, true);
            return pline;
        }

        private Polyline3d getRouteSharp(Tool tool, Routing route)
        {
            double height = 400;//设定一个高度  
            Point3d pt1 = new Point3d(tool.Diameter / 2, 0, 0);
            Point3d pt2 = new Point3d(-tool.Diameter / 2, 0, 0);
            Point3d pt3 = new Point3d(-tool.Diameter / 2, height, 0);
            Point3d pt4 = new Point3d(tool.Diameter / 2, height, 0);
            Point3d pt5 = new Point3d(tool.Diameter / 2, 0, 0);
            Point3dCollection pts = new Point3dCollection() { pt1, pt2, pt3, pt4, pt5 };
            Polyline3d pline = new Polyline3d(Poly3dType.SimplePoly, pts, true);
            return pline;
        }

        void moveSolidToDepth(Routing Route, Solid3d pathSolid, double depth)
        {
            if (Route.OnTopFace)
            {
                pathSolid.TransformBy(Matrix3d.Displacement(new Vector3d(0, 0, Route.Part.Thickness / 2 - depth)));
            }
            else
            {
                pathSolid.TransformBy(Matrix3d.Displacement(new Vector3d(0, 0, -Route.Part.Thickness / 2 + depth)));
            }
        }

        void addToDrawing(Entity entity)
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)acTrans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                btr.AppendEntity(entity);
                acTrans.AddNewlyCreatedDBObject(entity, true);

                acTrans.Commit();
            }
        }
    }
}
