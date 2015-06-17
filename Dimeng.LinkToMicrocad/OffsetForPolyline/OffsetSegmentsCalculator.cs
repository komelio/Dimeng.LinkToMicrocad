using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offset
{
    public class OffsetSegmentsCalculator
    {
        public static List<Curve> GetValue(Polyline polyline, double offsetDist, bool isLeft)
        {
            var line = (new PolylineConverter()).ConverToMyPolyline(polyline);
            OffsetSegmentsCollection ofsc = new OffsetSegmentsCollection(line, offsetDist);

            List<Curve> curves = new List<Curve>();

            if (isLeft)
            {
                ofsc.GetOffsetFinalResult(ofsc.pline1, ofsc.pline2, offsetDist);
            }
            else
            {
                ofsc.GetOffsetFinalResult(ofsc.pline2, ofsc.pline1, offsetDist);
            }

            for (int i = 0; i < ofsc.plinetodrawing.Count - 1; i++)
            {
                SEG noseg = ofsc.plinetodrawing[i].seg;
                Point2d nosegep = ofsc.plinetodrawing[i].endOfSeg;
                Point3d nops = P2dTo3d(noseg.point);
                Point3d nope = P2dTo3d(nosegep);

                if (Math.Abs(noseg.bulge) < 0.001)
                {
                    curves.Add(new Line(nops, nope));
                }
                else
                {
                    CircleEquation nCirseg = new CircleEquation(noseg, nosegep);
                    Point2d cen = new Point2d(nCirseg.a, nCirseg.b);
                    if (noseg.bulge > 0)
                    {
                        curves.Add(GeometryCalculations.GetArcFrom3Point2d(cen, noseg.point, nosegep));
                    }
                    else
                    {
                        curves.Add(GeometryCalculations.GetArcFrom3Point2d(cen, nosegep, noseg.point));
                    }
                }
            }

            return curves;
        }

        private static Point3d P2dTo3d(Point2d point2d)
        {
            return new Point3d(point2d.X, point2d.Y, 0);
        }
    }
}
