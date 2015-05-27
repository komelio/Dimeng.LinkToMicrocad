using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.DatabaseServices;

namespace Dimeng.WoodEngine.Math
{
    public class MathHelper
    {
        public static double GetAngleByLengthAndRadius(double Length, double radius)
        {
            if (radius == 0)
                throw new System.Exception("半径不能为0");

            return Length / radius;
        }

        public static Vector3d GetRotateVector(Vector3d v, double xr, double yr, double zr)
        {
            v = v.TransformBy(Matrix3d.Rotation(xr * System.Math.PI / 180, Vector3d.XAxis, Point3d.Origin));
            v = v.TransformBy(Matrix3d.Rotation(yr * System.Math.PI / 180, Vector3d.YAxis, Point3d.Origin));
            v = v.TransformBy(Matrix3d.Rotation(zr * System.Math.PI / 180, Vector3d.ZAxis, Point3d.Origin));
            return v;
        }

        public static Point3d GetRotatedAndMovedPoint(Point3d p, double xr, double yr, double zr, Vector3d moveV)
        {
            p = p.TransformBy(Matrix3d.Rotation(xr * System.Math.PI / 180, Vector3d.XAxis, Point3d.Origin));
            p = p.TransformBy(Matrix3d.Rotation(yr * System.Math.PI / 180, Vector3d.YAxis, Point3d.Origin));
            p = p.TransformBy(Matrix3d.Rotation(zr * System.Math.PI / 180, Vector3d.ZAxis, Point3d.Origin));
            p = p.TransformBy(Matrix3d.Displacement(moveV));
            return p;
        }

        public static List<int> FindSelfIntersectPline(Polyline polyline)
        {
            List<int> resultPts = new List<int>();

            DBObjectCollection entities = new DBObjectCollection();
            polyline.Explode(entities);

            for (int i = 0; i < entities.Count; ++i)
            {
                for (int j = i + 1; j < entities.Count; ++j)
                {
                    Curve curve1 = entities[i] as Curve;
                    Curve curve2 = entities[j] as Curve;

                    Autodesk.AutoCAD.Geometry.Point3dCollection points = new Autodesk.AutoCAD.Geometry.Point3dCollection();
                    curve1.IntersectWith(
                        curve2,
                        Intersect.OnBothOperands,
                        points,
                        IntPtr.Zero,
                        IntPtr.Zero);

                    foreach (Point3d point in points)
                    {
                        // Make a check to skip the start/end points
                        // since they are connected vertices
                        if (point == curve1.StartPoint ||
                            point == curve1.EndPoint)
                        {
                            if (point == curve2.StartPoint ||
                                point == curve2.EndPoint)
                            {
                                // If two consecutive segments, then skip
                                if (j == i + 1)
                                {
                                    continue;
                                }
                            }
                        }

                        resultPts.Add(j);
                    }
                }

                // Need to be disposed explicitely
                // since entities are not DB resident
                entities[i].Dispose();
            }
            return resultPts;
        }

        public static Polyline GetOffsetPolyline(Polyline line, bool IsLeftComp, double r)
        {
            Polyline newLine = new Polyline();
            double offsetValue = (IsLeftComp) ? -r : r;

            for (int i = 1; i <= line.NumberOfVertices - 1; i++)
            {
                Polyline pl1 = new Polyline();
                pl1.AddVertexAt(0, line.GetPoint2dAt(i - 1), line.GetBulgeAt(i - 1), 0, 0);
                pl1.AddVertexAt(1, line.GetPoint2dAt(i), line.GetBulgeAt(i), 0, 0);

                Polyline pl1Offset = pl1.GetOffsetCurves(offsetValue)[0] as Polyline;
                AddToDrawing(pl1Offset);//debug

                if (i == 1)//第一点，且当前曲线并不是封闭的
                {
                    newLine.AddVertexAt(0, new Point2d(pl1Offset.StartPoint.X, pl1Offset.StartPoint.Y), 0, 0, 0);
                    if (line.NumberOfVertices == 2)//如果只是一条线的话
                    {
                        newLine.AddVertexAt(0, new Point2d(pl1Offset.EndPoint.X, pl1Offset.EndPoint.Y), -pl1Offset.GetBulgeAt(0), 0, 0);
                    }
                }

                if (line.NumberOfVertices > 2 && i < line.NumberOfVertices - 1)
                {
                    Polyline pl2 = new Polyline();
                    pl2.AddVertexAt(0, line.GetPoint2dAt(i), line.GetBulgeAt(i), 0, 0);
                    pl2.AddVertexAt(1, line.GetPoint2dAt(i + 1), line.GetBulgeAt(i + 1), 0, 0);

                    Polyline pl2Offset = pl2.GetOffsetCurves(offsetValue)[0] as Polyline;
                    AddToDrawing(pl2Offset);//debug

                    //两个偏移后的Polyline进行相交
                    Point3dCollection points = new Point3dCollection();
                    pl2Offset.IntersectWith(pl1Offset, Intersect.ExtendBoth, points, IntPtr.Zero, IntPtr.Zero);

                    Point2d TheIntersectPoint;
                    if (points.Count == 0)
                    {
                        //无交点，只存在于两个在同一根直线上的情况
                        //或者同一个圆上
                        newLine.AddVertexAt(0, pl1Offset.GetPoint2dAt(1), -pl1Offset.GetBulgeAt(1), 0, 0);
                        continue;
                    }
                    else if (points.Count == 1)
                        TheIntersectPoint = new Point2d(points[0].X, points[0].Y);//1个交点，说明是直线和直线相交
                    else
                    {
                        //2个交点，那就需要判断是哪一个了
                        //与pl2offset的终点进行比较，距离较近的那个就是了
                        double dist1 = points[0].DistanceTo(pl2.StartPoint);
                        double dist2 = points[1].DistanceTo(pl2.StartPoint);
                        if (dist1 > dist2)
                            TheIntersectPoint = new Point2d(points[1].X, points[1].Y);
                        else
                            TheIntersectPoint = new Point2d(points[0].X, points[0].Y);
                    }

                    double newBulge = GetOffsetCurveBulge.Get(IsLeftComp, r, line, TheIntersectPoint, i, pl1Offset.GetPoint2dAt(0));
                    newLine.AddVertexAt(0, TheIntersectPoint, newBulge, 0, 0);

                    if (i == line.NumberOfVertices - 2)//最后一个点的时候
                    {
                        double bulge = GetOffsetCurveBulge.Get(IsLeftComp, r, line, pl2Offset.GetPoint2dAt(1), i + 1, pl2Offset.GetPoint2dAt(0));
                        newLine.AddVertexAt(0, new Point2d(pl2Offset.EndPoint.X, pl2Offset.EndPoint.Y), bulge, 0, 0);
                    }

                    pl2.Dispose();
                    pl2Offset.Dispose();
                }

                pl1.Dispose();
                pl1Offset.Dispose();
            }
            ReversePolyline(newLine);
            // newLine.ReverseCurve();//反转多段线
            return newLine;
        }

        private static void AddToDrawing(Polyline pl)
        {

            //Autodesk.AutoCAD.ApplicationServices.Document acDoc
            //    = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            //Database acCurDb = acDoc.Database;

            //using (Transaction acTrans = acDoc.TransactionManager.StartTransaction())
            //{
            //    BlockTable bt = (BlockTable)acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead);
            //    BlockTableRecord btr = (BlockTableRecord)acTrans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
            //    btr.AppendEntity(pl);
            //    acTrans.AddNewlyCreatedDBObject(pl, true);
            //    acTrans.Commit();
            //}
        }

        public static void ReversePolyline(Polyline pl)
        {
            if (pl != null)
            {
                // Collect our per-vertex data

                List<PerVertexData> vertData =
                  new List<PerVertexData>(pl.NumberOfVertices);

                for (int i = 0; i < pl.NumberOfVertices; i++)
                {
                    PerVertexData pvd = new PerVertexData();
                    pvd.bulge = (i > 0 ? pl.GetBulgeAt(i - 1) : 0);
                    pvd.startWidth = (i > 0 ? pl.GetStartWidthAt(i - 1) : 0);
                    pvd.endWidth = (i > 0 ? pl.GetEndWidthAt(i - 1) : 0);
                    pvd.pt = pl.GetPoint2dAt(i);

                    vertData.Add(pvd);
                }

                // Write the data back to the polyline, but in
                // reverse order

                for (int i = 0; i < pl.NumberOfVertices; i++)
                {
                    PerVertexData pvd =
                      vertData[pl.NumberOfVertices - (i + 1)];
                    pl.SetPointAt(i, pvd.pt);
                    pl.SetBulgeAt(i, -pvd.bulge);
                    pl.SetStartWidthAt(i, pvd.endWidth);
                    pl.SetEndWidthAt(i, pvd.startWidth);
                }
            }
        }
    }

    struct PerVertexData
    {
        public Point2d pt;
        public double bulge;
        public double startWidth;
        public double endWidth;
    }
}

