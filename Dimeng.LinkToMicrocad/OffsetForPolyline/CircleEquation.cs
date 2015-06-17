using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

namespace Offset
{
    public class CircleEquation:Equation
    {
        public double D { get; private set; }

        public double E { get; private set; }

        public double F { get; private set; }

        public double a { get; private set; }

        public double b { get; private set; }

        public double r { get; private set; }

        public static CircleEquation GetStandardForm(double d, double e, double f)
        {
            CircleEquation circle = new CircleEquation();
            circle.D = d;
            circle.E = e;
            circle.F = f;
            circle.a = d / (-2);
            circle.b = e / (-2);
            circle.r = Math.Sqrt(circle.a * circle.a + circle.b * circle.b - f);
            return circle;
        }

        public static CircleEquation GetCircleCenterForm(double aa, double bb, double rr)
        {
            CircleEquation circle = new CircleEquation();
            circle.a = aa;
            circle.b = bb;
            circle.r = rr;
            circle.D = -2 * aa;
            circle.E = -2 * bb;
            circle.F = aa * aa + bb * bb - rr * rr;
            return circle;
        }

        public CircleEquation() { }

        public CircleEquation(SEG segment, Point2d end)
        {
            double segBulge = Math.Abs(segment.bulge);//拱高比的绝对值
            Vector2d chord = VectorCalculations.VectorOfTwoPoints(segment.point, end);//弧的弦向量,方向为Si的起点到其重点
            Vector2d pNormalOfChord = VectorCalculations.PositiveNormalVector(chord);//弦的法向量
            double chordLength = VectorCalculations.NormOfVector(chord);//弦长
            double arcHeight = segBulge * (chordLength / 2);//拱高
            Point2d midPChord = VectorCalculations.MidPoint(segment.point, end);//弦的中点
            double arcAngle = Math.Atan(segBulge) * 4;//弧所对的圆心角
            double y_T = Math.Tan(arcAngle / 2);
            double x_T = segBulge * y_T;
            Vector2d arcHei = arcHeight * VectorCalculations.unitVector(pNormalOfChord);
            Point2d center;
            if (segment.bulge > 0)
            {
                center = VectorCalculations.MoveOfPoint(midPChord, arcHei/x_T);//圆心
            }
            else
            {
                center = VectorCalculations.MoveOfPoint(midPChord, -arcHei/x_T);
            }
            a = center.X;
            b = center.Y;
            r = VectorCalculations.NormOfVector(VectorCalculations.VectorOfTwoPoints(segment.point, center));
            D = -2 * a;
            E = -2 * b;
            F = a * a + b * b - r * r;
        }

        public override bool HasIPWith(LineEquation line)
        {
            return line.HasIPWith(this);
        }

        public override bool HasIPWith(CircleEquation circle)
        {
            double distanceOfCenters = VectorCalculations.NormOfVector(VectorCalculations.VectorOfTwoPoints(new Point2d(a, b), new Point2d(circle.a, circle.b)));
            if (distanceOfCenters <= r + circle.r && distanceOfCenters >= Math.Abs(r - circle.r))
            {
                return true;
            }
            return false;
        }

        public bool IsOverlappingWith(CircleEquation circle)
        {
            if ((Math.Abs(a - circle.a) <= SomeConstants.ERRORLIMIT) && (Math.Abs(b - circle.b) <= SomeConstants.ERRORLIMIT) && (Math.Abs(r - circle.r) <= SomeConstants.ERRORLIMIT))
                return true;
            else
            {
                if ((Math.Abs(D - circle.D) <= SomeConstants.ERRORLIMIT) && (Math.Abs(E - circle.E) <= SomeConstants.ERRORLIMIT) && (Math.Abs(F - circle.F) <= SomeConstants.ERRORLIMIT))
                    return false;
            }
            return false;
        }

        public override Point2d[] IPWith(LineEquation line)
        {
            return line.IPWith(this);
        }

        public override Point2d[] IPWith(CircleEquation circle)
        {
            if (Math.Abs(this.E - circle.E) > SomeConstants.ERRORLIMIT)
            {
                double newa = (circle.D - D) / (E - circle.E);
                double newb = (circle.F - F) / (E - circle.E);
                LineEquation ILine = new LineEquation(newa, newb);
                return ILine.IPWith(this);
            }
            else
            {
                double newt = (circle.F - this.F) / (this.D - circle.D);
                LineEquation newline = new LineEquation(newt);
                return newline.IPWith(this);
            }
        }
    }
}
