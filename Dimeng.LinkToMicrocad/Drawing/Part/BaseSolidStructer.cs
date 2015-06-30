using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dimeng.LinkToMicrocad.Logging;
using Dimeng.WoodEngine.Entities;
using Dimeng.WoodEngine.Entities.Machinings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PolylineGetter;

namespace Dimeng.LinkToMicrocad.Drawing
{
    public class BaseSolidStructer
    {
        public Solid3d Draw(Part part)
        {
            //Solid3d solid = new Solid3d();
            //solid.CreateBox(part.Length, part.Width, part.Thickness);
            //return solid;

            Polyline routePLine = new Polyline();
            Polyline pline = getRectanglePolyline(part);
            //addToDrawing(pline);

            Logger.GetLogger().Info("Border Routings : " + part.Routings.Where(it => it.HasChangedPartRectangleBorder).Count().ToString());
            foreach (var route in part.Routings.Where(it => it.HasChangedPartRectangleBorder))
            {
                routePLine = getRoutePLine(route);

                //之前已经过滤没有刀补的铣型了
                bool isLeft = getRouteComp(route);

                //addToDrawing(pline);
                //addToDrawing(routePLine);

                if (routePLine.Normal.Z < 0)//会出现这种情况，是因为机加工原点在wcs的下方，导致MPZAxis是朝下的，这样画出来的bulge就会相反，所以要进行二次纠正
                {
                    Polyline newPline = new Polyline();

                    for (int i = 0; i < routePLine.NumberOfVertices; i++)
                    {
                        var pt = routePLine.GetPoint3dAt(i);
                        newPline.AddVertexAt(i, new Point2d(pt.X, pt.Y), -routePLine.GetBulgeAt(i), 0, 0);//注意bulge是负值
                    }
                    routePLine = newPline;
                    //addToDrawing(newPline);
                }

                PolylineGetter.PolylineGetter plineTrimer = new PolylineGetter.PolylineGetter();

                var lines = plineTrimer.CalculatePolyline(pline, routePLine, isLeft);

                if (lines.Count != 1)
                {
                    throw new Exception("计算刀补图形时发生错误");
                }

                for (int i = 1; i < lines.Count; i++)
                {
                    lines[i].Dispose();
                }

                pline = lines[0];
            }

            //todo:dispose
            //todo:自交和非闭合的判断
            try
            {
                Solid3d solidOut = new Solid3d();
                DBObjectCollection regs = new DBObjectCollection();
                regs.Add(pline);
                var regions = Region.CreateFromCurves(regs);

                using (Region region = (Region)regions[0])//重要！否则及其容易导致崩溃，内存错误等问题
                {
                    solidOut.Extrude(region, part.Thickness, 0);
                    solidOut.TransformBy(Matrix3d.Displacement(new Vector3d(0, 0, -part.Thickness / 2)));

                    for (int i = 1; i < regions.Count; i++)
                    {
                        regions[i].Dispose();
                    }

                    return solidOut;
                }
            }
            catch
            {
                //addToDrawing(pline);

                Solid3d panel = new Solid3d();
                panel.CreateBox(part.Length, part.Width, part.Thickness);
                return panel;
            }
            finally
            {
                pline.Dispose();
                routePLine.Dispose();
            }
        }

        private bool getRouteComp(Routing route)
        {
            bool isLeft = (route.ToolComp == ToolComp.Left) ? false : true;
            return isLeft;
        }

        private Polyline getRoutePLine(Routing route)
        {
            var line = new Polyline();
            for (int i = 0; i < route.Points.Count; i++)
            {
                //铣型多段线的点,以机加工原点为坐标系原点

                Point2d orignPt = new Point2d(route.Points[i].X, route.Points[i].Y);
                line.AddVertexAt(i,
                                  orignPt,
                                  -route.Bulges[i],
                                  0, 0);

            }

            line.TransformBy(Matrix3d.AlignCoordinateSystem(Point3d.Origin,
                                                                  Vector3d.XAxis,
                                                                  Vector3d.YAxis,
                                                                  Vector3d.ZAxis,
                                                                  new Point3d(route.Part.MPPoint.X, route.Part.MPPoint.Y, 0),
                                                                  route.Part.MPXAxis,
                                                                  route.Part.MPYAxis,
                                                                  route.Part.MPZAxis));
            return line;
        }

        private Polyline getRectanglePolyline(Part part)
        {
            Polyline pline = new Polyline();
            Point2d pt1 = new Point2d(-part.Length / 2, part.Width / 2);
            Point2d pt2 = new Point2d(part.Length / 2, part.Width / 2);
            Point2d pt3 = new Point2d(part.Length / 2, -part.Width / 2);
            Point2d pt4 = new Point2d(-part.Length / 2, -part.Width / 2);
            pline.AddVertexAt(0, pt1, 0, 0, 0);
            pline.AddVertexAt(0, pt2, 0, 0, 0);
            pline.AddVertexAt(0, pt3, 0, 0, 0);
            pline.AddVertexAt(0, pt4, 0, 0, 0);
            pline.Closed = true;
            return pline;
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
