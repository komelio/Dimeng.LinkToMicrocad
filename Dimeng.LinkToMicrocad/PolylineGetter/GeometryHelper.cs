using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dimeng.LinkToMicrocad.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolylineGetter
{
    public class GeometryHelper
    {
        public static List<Point3d> GetIntersectPoints(Polyline line1, Polyline line2)
        {
            List<Point3d> points = new List<Point3d>();

            DBObjectCollection curves1 = new DBObjectCollection();
            DBObjectCollection curves2 = new DBObjectCollection();

            line1.Explode(curves1);
            line2.Explode(curves2);

            foreach (Curve kCurve in curves2)
            {
                foreach (Curve bCurve in curves1)
                {
                    if (kCurve is Line && bCurve is Line)
                    {
                        using (LineSegment3d kLine = new LineSegment3d(kCurve.StartPoint, kCurve.EndPoint))
                        using (LineSegment3d bLine = new LineSegment3d(bCurve.StartPoint, bCurve.EndPoint))
                        {
                            if (kLine.IsColinearTo(bLine, new Tolerance()))
                            {
                                LinearEntity3d l3 = kLine.Overlap(bLine);

                                if (l3 != null)
                                {
                                    addPointToCollection(l3.StartPoint, points);
                                    addPointToCollection(l3.EndPoint, points);
                                }
                            }
                            else
                            {
                                Point3dCollection pts = new Point3dCollection();
                                bCurve.IntersectWith(kCurve, Intersect.OnBothOperands, pts, IntPtr.Zero, IntPtr.Zero);

                                foreach (Point3d pt in pts)
                                {
                                    addPointToCollection(pt, points);
                                }
                            }
                        }
                    }
                    else
                    {
                        Point3dCollection pts = new Point3dCollection();
                        bCurve.IntersectWith(kCurve, Intersect.OnBothOperands, pts, IntPtr.Zero, IntPtr.Zero);

                        foreach (Point3d pt in pts)
                        {
                            addPointToCollection(pt, points);
                        }
                    }
                }
            }

            //dispose
            foreach (Entity e in curves1)
            {
                e.Dispose();
            }
            foreach (Entity e in curves2)
            {
                e.Dispose();
            }
            curves1.Dispose();
            curves2.Dispose();

            foreach (var pt in points)
            {
                Logger.GetLogger().Debug("Interset Point:" + pt.ToString());
            }

            return points;
        }

        private static void addPointToCollection(Point3d pt, List<Point3d> points)
        {
            var x = points.Find(it => it.DistanceTo(pt) <= 0.001);
            if (x == Point3d.Origin)
            {
                points.Add(pt);
            }
        }
    }
}
