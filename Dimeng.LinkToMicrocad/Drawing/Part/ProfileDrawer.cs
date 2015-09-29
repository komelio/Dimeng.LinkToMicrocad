using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dimeng.LinkToMicrocad.Drawing.CAD;
using Dimeng.LinkToMicrocad.Logging;
using Dimeng.WoodEngine.Entities;
using Dimeng.WoodEngine.Entities.Machinings;
using Dimeng.WoodEngine.Math;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dimeng.LinkToMicrocad.Drawing
{
    public class ProfileDrawer
    {
        string graphicPath;
        Database db;
        public ProfileDrawer(string graphicPath, Database db)
        {
            this.db = db;
            this.graphicPath = graphicPath;
        }

        public void Draw(ObjectId panelId, Part part)
        {
            foreach (var profile in part.Profiles)
            {
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);

                    Solid3d panel = trans.GetObject(panelId, OpenMode.ForWrite) as Solid3d;

                    Polyline section;//截面

                    try
                    {
                        if (profile.IsSharpFromFile)
                        {
                            string file = graphicPath;
                            file = Path.Combine(file, profile.SharpFile);

                            if (!File.Exists(file))
                            {
                                Logger.GetLogger().Warn(string.Format("Profile dwg file {0} not found!", file));
                                return;
                                throw new FileNotFoundException(file + " not found!!!!!!By profile drawer");
                            }

                            Entity sectionOld = CADHelper.AddDwgEntities(file, trans, db);
                            section = CADHelper.RebuildPolyline(sectionOld, db);
                            sectionOld.Erase();//删去旧的
                            //btr.AppendEntity(section);
                            //trans.AddNewlyCreatedDBObject(section, true);

                            //Polyline pl = section as Polyline;
                            //var points = Dimeng.Math.MathHelper.FindSelfIntersectPline(pl);
                            //if (points.Count > 0)
                            //{

                            //    StringBuilder sb = new StringBuilder();
                            //    foreach (int i in points)
                            //    {
                            //        sb.Append(i);
                            //        sb.Append(",");
                            //    }
                            //    Logger.GetLogger().Warn("Section is intersectSelf!" + sb.ToString());
                            //    //section.Erase();
                            //    section.Dispose();
                            //    continue;
                            //}

                        }
                        else
                        {
                            //否则就直接绘制出来,目前只针对绘制三角形的情况——针对MITER这个指令                        
                            //这是个倒三角
                            Polyline polyline = new Polyline();
                            Point2d pt1 = new Point2d(-0.1, 0);
                            Point2d pt2 = new Point2d(part.Thickness / System.Math.Tan(profile.Angle), part.Thickness);
                            Point2d pt3 = new Point2d(-0.1, part.Thickness);
                            polyline.AddVertexAt(0, pt1, 0, 0, 0);
                            polyline.AddVertexAt(1, pt2, 0, 0, 0);
                            polyline.AddVertexAt(2, pt3, 0, 0, 0);
                            polyline.Closed = true;
                            section = polyline;
                        }

                        Point3d startPt, endPt;
                        RotateSection(profile, section, part, out startPt, out endPt);//TODO:这个Section的旋转有问题，有可能会无法扫掠成功

                        Line line = new Line(startPt, endPt);//扫掠的路径
                        line.TransformBy(Matrix3d.Displacement(section.StartPoint - line.StartPoint));

                        //btr.AppendEntity(line);
                        //trans.AddNewlyCreatedDBObject(line, true);
                        //btr.AppendEntity(section);
                        //trans.AddNewlyCreatedDBObject(section, true);


                        Solid3d proSolid = new Solid3d();
                        SweepOptionsBuilder sob = new SweepOptionsBuilder();
                        sob.Align = SweepOptionsAlignOption.AlignSweepEntityToPath;
                        sob.BasePoint = line.StartPoint;
                        sob.Bank = true;

                        proSolid.CreateSweptSolid(section, line, sob.ToSweepOptions());
                        panel.BooleanOperation(BooleanOperationType.BoolSubtract, proSolid);

                        line.Dispose();
                        proSolid.Dispose();
                        //section.Erase();
                        section.Dispose();

                        //btr.AppendEntity(proSolid);
                        //trans.AddNewlyCreatedDBObject(proSolid, true);
                        trans.Commit();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        private static void RotateSection(Profile profile, Entity section, Part part, out Point3d startPt, out Point3d endPt)
        {
            startPt = part.GetPartPointByNumber(profile.StartPointNumber.ToString());
            endPt = part.GetPartPointByNumber(profile.EndPointNumber.ToString());

            int gap = System.Math.Abs(profile.StartPointNumber - profile.EndPointNumber);
            if (gap == 2 || gap == 6)
            {
                bool longSide = true;//沿着板件的长度方向    
                if (startPt.X == endPt.X)//判断起点的终点的X是否相同       
                { longSide = false; }
                else { longSide = true; }

                Vector3d xaxis, yaxis, zaxis;
                if (longSide)
                {
                    GetXYAxisByPointNumberLongSide(profile.StartPointNumber.ToString(), out xaxis, out yaxis, out zaxis);
                }
                else
                {
                    GetXYAxisByPointNumberWidthSide(profile.StartPointNumber.ToString(), out xaxis, out yaxis, out zaxis);
                }

                section.TransformBy(Matrix3d.AlignCoordinateSystem(
                    Point3d.Origin,
                    Vector3d.XAxis,
                    Vector3d.YAxis,
                    Vector3d.ZAxis,
                    part.GetPartPointByNumber(profile.StartPointNumber.ToString()),
                    xaxis,
                    yaxis,
                    zaxis));
                return;
            }
            else if (gap == 1)
            {
                Vector3d xaxis, yaxis, zaxis;
                GetXYAxisByPointNumberThickSide(profile.StartPointNumber.ToString(), out xaxis, out yaxis, out zaxis);
                section.TransformBy(Matrix3d.AlignCoordinateSystem(
                    Point3d.Origin,
                    Vector3d.XAxis,
                    Vector3d.YAxis,
                    Vector3d.ZAxis,
                    part.GetPartPointByNumber(profile.StartPointNumber.ToString()),
                    xaxis,
                    yaxis,
                    zaxis));
                return;
            }

            throw new Exception(string.Format("Unknown profile start point and end point!{0}/{1}", profile.StartPointNumber, profile.EndPointNumber));
        }

        private static void GetXYAxisByPointNumberLongSide(string ptNum, out Vector3d xaxis, out Vector3d yaxis, out Vector3d zaxis)
        {
            switch (ptNum)
            {
                case "1":
                    xaxis = Vector3d.YAxis;
                    yaxis = Vector3d.ZAxis;
                    zaxis = Vector3d.XAxis;
                    break;
                case "2":
                    xaxis = Vector3d.YAxis;
                    yaxis = -Vector3d.ZAxis;
                    zaxis = Vector3d.XAxis;
                    break;
                case "3":
                    xaxis = -Vector3d.YAxis;
                    yaxis = Vector3d.ZAxis;
                    zaxis = Vector3d.XAxis;
                    break;
                case "4":
                    xaxis = -Vector3d.YAxis;
                    yaxis = -Vector3d.ZAxis;
                    zaxis = Vector3d.XAxis;
                    break;
                case "5":
                    xaxis = Vector3d.YAxis;
                    yaxis = Vector3d.ZAxis;
                    zaxis = -Vector3d.XAxis;
                    break;
                case "6":
                    xaxis = Vector3d.YAxis;
                    yaxis = -Vector3d.ZAxis;
                    zaxis = -Vector3d.XAxis;
                    break;
                case "7":
                    xaxis = -Vector3d.YAxis;
                    yaxis = Vector3d.ZAxis;
                    zaxis = -Vector3d.XAxis;
                    break;
                case "8":
                    xaxis = -Vector3d.YAxis;
                    yaxis = -Vector3d.ZAxis;
                    zaxis = -Vector3d.XAxis;
                    break;
                default:
                    throw new System.Exception("未知的点:" + ptNum);
            }
        }

        private static void GetXYAxisByPointNumberWidthSide(string ptNum, out Vector3d xaxis, out Vector3d yaxis, out Vector3d zaxis)
        {
            switch (ptNum)
            {
                case "1":
                    xaxis = Vector3d.XAxis;
                    yaxis = Vector3d.ZAxis;
                    zaxis = Vector3d.YAxis;
                    break;
                case "2":
                    xaxis = Vector3d.XAxis;
                    yaxis = -Vector3d.ZAxis;
                    zaxis = Vector3d.YAxis;
                    break;
                case "3":
                    xaxis = Vector3d.XAxis;
                    yaxis = Vector3d.ZAxis;
                    zaxis = -Vector3d.YAxis;
                    break;
                case "4":
                    xaxis = -Vector3d.XAxis;
                    yaxis = -Vector3d.ZAxis;
                    zaxis = -Vector3d.YAxis;
                    break;
                case "5":
                    xaxis = -Vector3d.XAxis;
                    yaxis = Vector3d.ZAxis;
                    zaxis = -Vector3d.YAxis;
                    break;
                case "6":
                    xaxis = -Vector3d.XAxis;
                    yaxis = -Vector3d.ZAxis;
                    zaxis = -Vector3d.YAxis;
                    break;
                case "7":
                    xaxis = -Vector3d.XAxis;
                    yaxis = Vector3d.ZAxis;
                    zaxis = Vector3d.YAxis;
                    break;
                case "8":
                    xaxis = -Vector3d.XAxis;
                    yaxis = -Vector3d.ZAxis;
                    zaxis = Vector3d.YAxis;
                    break;
                default:
                    throw new System.Exception("未知的点:" + ptNum);
            }
        }

        private static void GetXYAxisByPointNumberThickSide(string ptNum, out Vector3d xaxis, out Vector3d yaxis, out Vector3d zaxis)
        {
            switch (ptNum)
            {
                case "1":
                    xaxis = Vector3d.XAxis;
                    yaxis = Vector3d.YAxis;
                    zaxis = Vector3d.ZAxis;
                    break;
                case "2":
                    xaxis = Vector3d.XAxis;
                    yaxis = Vector3d.YAxis;
                    zaxis = -Vector3d.ZAxis;
                    break;
                case "3":
                    xaxis = Vector3d.XAxis;
                    yaxis = -Vector3d.YAxis;
                    zaxis = Vector3d.ZAxis;
                    break;
                case "4":
                    xaxis = Vector3d.XAxis;
                    yaxis = -Vector3d.YAxis;
                    zaxis = -Vector3d.ZAxis;
                    break;
                case "5":
                    xaxis = -Vector3d.XAxis;
                    yaxis = -Vector3d.YAxis;
                    zaxis = Vector3d.ZAxis;
                    break;
                case "6":
                    xaxis = -Vector3d.XAxis;
                    yaxis = -Vector3d.YAxis;
                    zaxis = -Vector3d.ZAxis;
                    break;
                case "7":
                    xaxis = -Vector3d.XAxis;
                    yaxis = Vector3d.YAxis;
                    zaxis = Vector3d.ZAxis;
                    break;
                case "8":
                    xaxis = -Vector3d.XAxis;
                    yaxis = Vector3d.YAxis;
                    zaxis = -Vector3d.ZAxis;
                    break;
                default:
                    throw new System.Exception("未知的点:" + ptNum);
            }
        }
    }
}
