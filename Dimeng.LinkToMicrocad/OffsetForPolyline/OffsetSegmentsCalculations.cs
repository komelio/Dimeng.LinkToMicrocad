using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using System.Collections;
using System.Collections.Generic;

namespace Offset
{
    /// <summary>
    /// 进行各段的偏移的相关的几何计算
    /// </summary>
    public static class OffsetSegmentsCalculations
    {


        public static SEG GetPositiveLineSegmentOffset(Point2d p1, Point2d p2, double offsetDistance, out Point2d endofseg)
        {
            Vector2d parellelVector = new Vector2d(p2.X - p1.X, p2.Y - p1.Y);
            Vector2d orthogonalVector = PositiveOrthogonalOfVector2d(parellelVector);
            Vector2d unitNormalOrthogonalVector = UnitNormalOfVector2d(orthogonalVector);
            Point2d lineSegStart2d = MoveOfPoint2d(p1, unitNormalOrthogonalVector, offsetDistance);
            Point2d lineSegEnd2d = MoveOfPoint2d(p2, unitNormalOrthogonalVector, offsetDistance);
            Point3d lineSegStart3d = Point2dToPoint3d(lineSegStart2d);
            Point3d lineSegEnd3d = Point2dToPoint3d(lineSegEnd2d);
            Line lineOffsetSegment = new Line(lineSegStart3d, lineSegEnd3d);
            endofseg = lineSegEnd2d;
            SEG seg = new SEG(lineSegStart2d, 0);
            return seg;
        }

        public static SEG GetNegativeLineSegmentOffset(Point2d p1, Point2d p2, double offsetDistance, out Point2d endofseg)
        {
            Vector2d parellelVector = new Vector2d(p2.X - p1.X, p2.Y - p1.Y);
            Vector2d orthogonalVector = NegativeOrthogonalOfVector2d(parellelVector);
            Vector2d unitNormalOrthogonalVector = UnitNormalOfVector2d(orthogonalVector);
            Point2d lineSegStart2d = MoveOfPoint2d(p1, unitNormalOrthogonalVector, offsetDistance);
            Point2d lineSegEnd2d = MoveOfPoint2d(p2, unitNormalOrthogonalVector, offsetDistance);
            Point3d lineSegStart3d = Point2dToPoint3d(lineSegStart2d);
            Point3d lineSegEnd3d = Point2dToPoint3d(lineSegEnd2d);
            Line lineOffsetSegment = new Line(lineSegStart3d, lineSegEnd3d);
            endofseg = lineSegEnd2d;
            SEG seg = new SEG(lineSegStart2d, 0);
            return seg;
        }

        public static SEG GetPositiveArcSegmentOffset(SEG seg1, SEG seg2, double offsetDistance, out Point2d endofseg)
        {
            double centralAngle = Math.Atan(Math.Abs(seg1.bulge)) * 4;//圆心角
            Vector2d chord = new Vector2d(seg2.point.X - seg1.point.X, seg2.point.Y - seg1.point.Y);//弦的方向向量
            double chordLength = NormOfVector2d(chord);//弦长
            double arcHeight = seg1.bulge * (chordLength / 2);//拱高
            Vector2d orthogonalVectorOfChord;//弦的法向量
            if (seg1.bulge < 0)
            {
                orthogonalVectorOfChord = PositiveOrthogonalOfVector2d(chord);
            }
            else
            {
                orthogonalVectorOfChord = NegativeOrthogonalOfVector2d(chord);
            }
            Point2d midPoint2dOfChord = MidPoint2d(seg1.point, seg2.point);//弦的中点

            Point2d midPoint2dOfArc = MoveOfPoint2d(midPoint2dOfChord, orthogonalVectorOfChord, arcHeight);//圆弧段的中点
            Vector2d arcHeightVector = TheVectorOfTwoPoint2ds(midPoint2dOfArc, midPoint2dOfChord);//指向圆心的向量
            Vector2d midPoint2dOfChordToCenter = arcHeightVector / (seg1.bulge * Math.Tan(centralAngle / 2));
            Point2d center = MoveOfPoint2d(midPoint2dOfChord, midPoint2dOfChordToCenter);//圆心
            Vector2d orthogonalStartVectorOfTangentVector = TheVectorOfTwoPoint2ds(seg1.point, center);
            Vector2d orthogonalEndVectorOfTangentVector = TheVectorOfTwoPoint2ds(seg2.point, center);
            Point2d startArcSegPoint = MoveOfPoint2d(seg1.point, orthogonalStartVectorOfTangentVector, offsetDistance);
            Point2d endArcSegPoint = MoveOfPoint2d(seg2.point, orthogonalEndVectorOfTangentVector, offsetDistance);
            SEG seg = new SEG(startArcSegPoint, seg1.bulge);
            endofseg = endArcSegPoint;
            if (seg1.bulge < 0)
            {
                orthogonalStartVectorOfTangentVector = TheVectorOfTwoPoint2ds(center, seg2.point);
                orthogonalEndVectorOfTangentVector = TheVectorOfTwoPoint2ds(center, seg1.point);
                startArcSegPoint = MoveOfPoint2d(seg2.point, orthogonalStartVectorOfTangentVector, offsetDistance);
                endArcSegPoint = MoveOfPoint2d(seg1.point, orthogonalEndVectorOfTangentVector, offsetDistance);
                seg = new SEG(endArcSegPoint, seg1.bulge);
                endofseg = startArcSegPoint;
            }
            return seg;
        }

        public static SEG GetNegativeArcSegmentOffset(SEG seg1, SEG seg2, double offsetDistance, out Point2d endofseg)
        {
            double centralAngle = Math.Atan(Math.Abs(seg1.bulge)) * 4;//圆心角
            Vector2d chord = new Vector2d(seg2.point.X - seg1.point.X, seg2.point.Y - seg1.point.Y);//弦的方向向量
            double chordLength = NormOfVector2d(chord);//弦长
            double arcHeight = seg1.bulge * (chordLength / 2);//拱高
            Vector2d orthogonalVectorOfChord;//弦的法向量
            if (seg1.bulge < 0)
            {
                orthogonalVectorOfChord = PositiveOrthogonalOfVector2d(chord);
            }
            else
            {
                orthogonalVectorOfChord = NegativeOrthogonalOfVector2d(chord);
            }
            Point2d midPoint2dOfChord = MidPoint2d(seg1.point, seg2.point);//弦的中点

            Point2d midPoint2dOfArc = MoveOfPoint2d(midPoint2dOfChord, orthogonalVectorOfChord, arcHeight);//圆弧段的中点
            Vector2d arcHeightVector = TheVectorOfTwoPoint2ds(midPoint2dOfArc, midPoint2dOfChord);//指向圆心的向量
            Vector2d midPoint2dOfChordToCenter = arcHeightVector / (seg1.bulge * Math.Tan(centralAngle / 2));
            Point2d center = MoveOfPoint2d(midPoint2dOfChord, midPoint2dOfChordToCenter);//圆心
            Vector2d orthogonalStartVectorOfTangentVector = TheVectorOfTwoPoint2ds(center, seg1.point);
            Vector2d orthogonalEndVectorOfTangentVector = TheVectorOfTwoPoint2ds(center, seg2.point);
            Point2d startArcSegPoint = MoveOfPoint2d(seg1.point, orthogonalStartVectorOfTangentVector, offsetDistance);
            Point2d endArcSegPoint = MoveOfPoint2d(seg2.point, orthogonalEndVectorOfTangentVector, offsetDistance);
            SEG seg = new SEG(startArcSegPoint, seg1.bulge);
            endofseg = endArcSegPoint;
            if (seg1.bulge < 0)
            {
                orthogonalStartVectorOfTangentVector = TheVectorOfTwoPoint2ds(seg2.point, center);
                orthogonalEndVectorOfTangentVector = TheVectorOfTwoPoint2ds(seg1.point, center);
                startArcSegPoint = MoveOfPoint2d(seg2.point, orthogonalStartVectorOfTangentVector, offsetDistance);
                endArcSegPoint = MoveOfPoint2d(seg1.point, orthogonalEndVectorOfTangentVector, offsetDistance);
                seg = new SEG(endArcSegPoint, seg1.bulge);
                endofseg = startArcSegPoint;

            }
            return seg;

        }

        public static double NormOfVector2d(Vector2d vector)
        {
            return Math.Sqrt(Math.Pow(vector.X, 2) + Math.Pow(vector.Y, 2));
        }

        private static Vector2d PositiveOrthogonalOfVector2d(Vector2d vector)
        {
            return new Vector2d(-(vector.Y), vector.X);
        }

        private static Vector2d NegativeOrthogonalOfVector2d(Vector2d vector)
        {
            return new Vector2d((vector.Y), -(vector.X));
        }

        private static Vector2d UnitNormalOfVector2d(Vector2d vector)
        {
            return new Vector2d(vector.X / NormOfVector2d(vector), vector.Y / NormOfVector2d(vector));
        }

        private static Point2d MoveOfPoint2d(Point2d point, Vector2d vector, double distance)
        {
            vector = UnitNormalOfVector2d(vector);
            return new Point2d(point.X + vector.X * distance, point.Y + vector.Y * distance);
        }

        private static Point2d MoveOfPoint2d(Point2d point, Vector2d vector)
        {
            return new Point2d(point.X + vector.X, point.Y + vector.Y);
        }

        private static Point3d Point2dToPoint3d(Point2d point2d)
        {
            return new Point3d(point2d.X, point2d.Y, 0);
        }

        public static Point2d MidPoint2d(Point2d p1, Point2d p2)
        {
            return new Point2d((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
        }

        private static Vector2d TheVectorOfTwoPoint2ds(Point2d p1, Point2d p2)
        {
            return new Vector2d(p2.X - p1.X, p2.Y - p1.Y);
        }

        private static Arc GetArcFrom3Point2d(Point2d center, Point2d start, Point2d end)
        {
            Vector2d vecStart = TheVectorOfTwoPoint2ds(center, start);
            Vector2d vecEnd = TheVectorOfTwoPoint2ds(center, end);
            double startAng = GetAngleOfVector2d(vecStart);
            double endAng = GetAngleOfVector2d(vecEnd);
            double radius = NormOfVector2d(TheVectorOfTwoPoint2ds(center, start));
            return new Arc(Point2dToPoint3d(center), radius, startAng, endAng);
        }

        private static double GetAngleOfVector2d(Vector2d vector)
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
                vVector = NormOfVector2d(vector);
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
                vVector = NormOfVector2d(vector);
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
                vVector = NormOfVector2d(vector);
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
                vVector = NormOfVector2d(vector);
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


        public static Point2d GetCenterOfArc(SEG seg1, Point2d endP)
        {
            double centralAngle = Math.Atan(Math.Abs(seg1.bulge)) * 4;//圆心角
            Vector2d chord = new Vector2d(endP.X - seg1.point.X, endP.Y - seg1.point.Y);//弦的方向向量
            double chordLength = NormOfVector2d(chord);//弦长
            double arcHeight = seg1.bulge * (chordLength / 2);//拱高
            Vector2d orthogonalVectorOfChord;//弦的法向量
            if (seg1.bulge < 0)
            {
                orthogonalVectorOfChord = PositiveOrthogonalOfVector2d(chord);
            }
            else
            {
                orthogonalVectorOfChord = NegativeOrthogonalOfVector2d(chord);
            }
            Point2d midPoint2dOfChord = MidPoint2d(seg1.point, endP);//弦的中点

            Point2d midPoint2dOfArc = MoveOfPoint2d(midPoint2dOfChord, orthogonalVectorOfChord, arcHeight);//圆弧段的中点
            Vector2d arcHeightVector = TheVectorOfTwoPoint2ds(midPoint2dOfArc, midPoint2dOfChord);//指向圆心的向量
            Vector2d midPoint2dOfChordToCenter = arcHeightVector / (seg1.bulge * Math.Tan(centralAngle / 2));
            Point2d center = MoveOfPoint2d(midPoint2dOfChord, midPoint2dOfChordToCenter);//圆心
            return center;
        }


    }
}
