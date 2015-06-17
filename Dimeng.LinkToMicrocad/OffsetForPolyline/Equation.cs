using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

namespace Offset
{
    public abstract class Equation
    {
        public abstract bool HasIPWith(LineEquation line);

        public abstract bool HasIPWith(CircleEquation circle);

        public abstract Point2d[] IPWith(LineEquation line);

        public abstract Point2d[] IPWith(CircleEquation circle);

        //public abstract bool IsOverlappingWith(Equation equation);

        //public abstract bool IsPointIn(Equation equation);

        public bool B2_4AC(double A, double B, double C)
        {
            return (B * B - 4 * A * C) >= 0||Math.Abs((B * B - 4 * A * C))<=SomeConstants.ERRORLIMIT;
        }

        public void GetX1X2(double A, double B, double C, out double X1, out double X2)
        {
            double sqrt_b2_4ac = Math.Sqrt(B * B - 4 * A * C);
            X1 = (-B + sqrt_b2_4ac) / (2 * A);
            X2 = (-B - sqrt_b2_4ac) / (2 * A);
        }
    }
}
