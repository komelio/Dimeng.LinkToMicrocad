using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

namespace Offset
{
    public static class VectorCalculations
    {
        public static double NormOfVector(Vector2d vec)
        {
            double douNorm = Math.Sqrt(vec.Y * vec.Y + vec.X * vec.X);
            return douNorm;
        }

        public static Vector2d VectorOfTwoPoints(Point2d start, Point2d end)
        {
            Vector2d vec = new Vector2d(end.X - start.X, end.Y - start.Y);
            return vec;
        }

        public static Vector2d unitVector(Vector2d vec)
        {
            double norm = NormOfVector(vec);
            Vector2d unitV = new Vector2d(vec.X / norm, vec.Y / norm);
            return unitV;
        }

        public static Vector2d PositiveUnitVector(Vector2d vec)
        {
            double norm = NormOfVector(vec);
            Vector2d unitV = new Vector2d(Math.Abs(vec.X) / norm, Math.Abs(vec.Y) / norm);
            return unitV;
        }

        public static Vector2d PositiveNormalVector(Vector2d vec)
        {
            Vector2d normalVector = new Vector2d(-vec.Y, vec.X);
            return normalVector;
        }

        public static Vector2d NegativeNormalVector(Vector2d vec)
        {
            Vector2d normalVector = new Vector2d(vec.Y, -vec.X);
            return normalVector;
        }

        public static Point2d MoveOfPoint(Point2d ori, Vector2d vec)
        {
            Point2d newp = new Point2d(ori.X + vec.X, ori.Y + vec.Y);
            return newp;
        }

        public static Point2d MidPoint(Point2d p1, Point2d p2)
        {
            Point2d midPoint = new Point2d((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
            return midPoint;
        }

        public static double GetAngleOfTwoVector(Vector2d v1, Vector2d v2)
        {
            double anglev1 = GetAngleOfVector(v1);
            double anglev2 = GetAngleOfVector(v2);
            return Math.Abs(anglev1 - anglev2);
        }

        public static double GetAngleOfVector(Vector2d vector)//与x轴正向的夹角
        {
            Vector2d vec1 = new Vector2d(1, 0);
            Vector2d vec2 = new Vector2d(0, 1);
            Vector2d vec3 = new Vector2d(-1, 0);
            Vector2d vec4 = new Vector2d(0, -1);
            double dotMultiple;
            double vVector;
            double cosAng;
            if (vector.X > 0 && vector.Y > 0)
            {
                dotMultiple = vec1.X * vector.X + vec1.Y * vector.Y;
                vVector = NormOfVector(vector);
                cosAng = dotMultiple / vVector;
                return Math.Acos(cosAng);
            }
            else if (vector.X == 0 && vector.Y > 0)
            {
                return Math.PI / 2;
            }
            else if (vector.X < 0 && vector.Y > 0)
            {
                dotMultiple = vec2.X * vector.X + vec2.Y * vector.Y;
                vVector = NormOfVector(vector);
                cosAng = dotMultiple / vVector;
                return Math.Acos(cosAng) + Math.PI / 2;
            }
            else if (vector.X < 0 && vector.Y == 0)
            {
                return Math.PI;
            }
            else if (vector.X < 0 && vector.Y < 0)
            {
                dotMultiple = vec3.X * vector.X + vec3.Y * vector.Y;
                vVector = NormOfVector(vector);
                cosAng = dotMultiple / vVector;
                return Math.Acos(cosAng) + Math.PI;
            }
            else if (vector.X == 0 && vector.Y < 0)
            {
                return Math.PI * 1.5;
            }
            else if (vector.X > 0 && vector.Y < 0)
            {
                dotMultiple = vec4.X * vector.X + vec4.Y * vector.Y;
                vVector = NormOfVector(vector);
                cosAng = dotMultiple / vVector;
                return Math.Acos(cosAng) + Math.PI * 1.5;
            }
            else if (vector.X > 0 && vector.Y == 0)
            {
                return Math.PI * 2;
            }
            else
            {
                return 0;
            }
        }

        public static Point2d GetVectorFromAngNorm(double ang, double norm, Point2d cir)
        {
            double xu = Math.Cos(ang);
            double yu = Math.Sin(ang);
            Vector2d p = new Vector2d(xu*norm, yu*norm);
            Point2d v=cir + p;
            return v;
        }
    }
}
