using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using System.Collections.Generic;

namespace Offset
{
    public static class PointWhere
    {
        public static bool IsPointInSegNotisstartandend(Point2d point, SEG seg, Point2d end)
        {
            if (seg.bulge == 0)
            {
                //LineEquation segLine = new LineEquation(seg, end);
                Vector2d SegLine = VectorCalculations.VectorOfTwoPoints(seg.point,end);//线段的方向向量,段的起点到终点
                Vector2d IPSegLine = VectorCalculations.VectorOfTwoPoints(seg.point,point);//段的起点到交点的方向向量
                if (Math.Abs(SegLine.X - IPSegLine.X) <= SomeConstants.ERRORLIMIT && Math.Abs(SegLine.Y - IPSegLine.Y) <= SomeConstants.ERRORLIMIT)
                    return false;
                Vector2d uvsl=VectorCalculations.unitVector(SegLine);
                Vector2d uvipsl=VectorCalculations.unitVector(IPSegLine);

                if ((Math.Abs(uvsl.X - uvipsl.X) <= SomeConstants.ERRORLIMIT) && (Math.Abs(uvipsl.Y - uvsl.Y) <= SomeConstants.ERRORLIMIT))
                {
                    double normsl = VectorCalculations.NormOfVector(SegLine);
                    double normipsl = VectorCalculations.NormOfVector(IPSegLine);
                    if (normipsl < normsl)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                CircleEquation segCircle = new CircleEquation(seg, end);
                Point2d circleCenter = new Point2d(segCircle.a, segCircle.b);
                Vector2d vcs = VectorCalculations.VectorOfTwoPoints(circleCenter, seg.point);
                Vector2d vce = VectorCalculations.VectorOfTwoPoints(circleCenter, end);
                Vector2d vcm = VectorCalculations.VectorOfTwoPoints(circleCenter, point);
                if (Math.Abs(vcm.X - vcs.X) <= SomeConstants.ERRORLIMIT && Math.Abs(vcm.Y - vcs.Y) <= SomeConstants.ERRORLIMIT)
                    return false;
                else if (Math.Abs(vcm.X - vce.X) <= SomeConstants.ERRORLIMIT && Math.Abs(vcm.Y - vce.Y) <= SomeConstants.ERRORLIMIT)
                    return false;
                double normvcs = VectorCalculations.NormOfVector(vcs);
                double normvcm = VectorCalculations.NormOfVector(vcm);
                double normvce = VectorCalculations.NormOfVector(vce);
                double angvce = VectorCalculations.GetAngleOfVector(vce);
                double angvcs = VectorCalculations.GetAngleOfVector(vcs);
                double angvcm = VectorCalculations.GetAngleOfVector(vcm);
                if ((Math.Abs(normvcm - normvcs) <= SomeConstants.ERRORLIMIT))
                {
                    if (seg.bulge < 0)
                    {
                        if (angvce < angvcs)
                        {
                            if (angvcm < angvcs && angvcm > angvce)
                                return true;
                        }
                        else
                        {
                            if (angvcm < angvcs)
                                return true;
                            if (angvcm > angvce)
                                return true;
                        }
                    }
                    else
                    {
                        if (angvce < angvcs)
                        {
                            if (angvcm > angvcs)
                                return true;
                            if (angvcm < angvce)
                                return true;
                        }
                        else
                        {
                            if (angvcm > angvcs && angvcm < angvce)
                                return true;
                        }
                    }
                }
                return false;
            }
        }

        public static bool IsPointInSegNotisstartandend(Point2d point, OffsetSegmentsCollection.offsetSegment seg)
        {
            return IsPointInSegNotisstartandend(point, seg.seg, seg.endOfSeg);
        }

        public static bool IsTheSamePoint(Point2d p1, Point2d p2)
        {
            return Math.Abs(p1.X - p2.X) <= SomeConstants.ERRORLIMIT && Math.Abs(p1.Y - p2.Y) <= SomeConstants.ERRORLIMIT;
        }

        public static bool HasLIP(SEG s1, Point2d s1end, SEG s2, Point2d s2end)
        {
            Point2d[] ips = null;
            if (s1.bulge == 0 && s2.bulge == 0)
            {
                LineEquation s1_line = new LineEquation(s1, s1end);
                LineEquation s2_line = new LineEquation(s2, s2end);

                if (s1_line.HasIPWith(s2_line)&&!s1_line.IsOverlappingWith(s2_line))
                {
                    ips = s1_line.IPWith(s2_line);
                }
                else
                {
                    return false;
                }
            }
            if (s1.bulge != 0 && s2.bulge == 0)
            {
                CircleEquation s1_circle = new CircleEquation(s1, s1end);
                LineEquation s2_line = new LineEquation(s2, s2end);

                if (s1_circle.HasIPWith(s2_line))
                {
                    ips = s1_circle.IPWith(s2_line);
                }
                else
                {
                    return false;
                }
            }
            if (s1.bulge != 0 && s2.bulge != 0)
            {
                CircleEquation s1_circle = new CircleEquation(s1, s1end);
                CircleEquation s2_circle = new CircleEquation(s2, s2end);

                if (s1_circle.HasIPWith(s2_circle)&&!s1_circle.IsOverlappingWith(s2_circle))
                {
                    ips = s1_circle.IPWith(s2_circle);
                }
                else
                {
                    return false;
                }
            }
            if (s2.bulge != 0 && s1.bulge == 0)
            {
                CircleEquation s2_circle = new CircleEquation(s2, s2end);
                LineEquation s1_line = new LineEquation(s1, s1end);

                if (s2_circle.HasIPWith(s1_line))
                {
                    ips = s2_circle.IPWith(s1_line);
                }
                else
                {
                    return false;
                }
            }
            foreach (Point2d ip in ips)
            {
                bool TIPforS1 = IsPointInSegNotisstartandend(ip, s1, s1end);
                bool TIPforS2 = IsPointInSegNotisstartandend(ip, s2, s2end);
                if (TIPforS1 && TIPforS2)
                    return true;
            }
            return false;
        }

        public static Point2d LIP(SEG s1, Point2d s1end, SEG s2, Point2d s2end)
        {
            Point2d[] ips = s1.GetIPS(s1end, s2, s2end);
            foreach (Point2d ip in ips)
            {
                bool TIPforS1 = IsPointInSegNotisstartandend(ip, s1, s1end);
                bool TIPforS2 = IsPointInSegNotisstartandend(ip, s2, s2end);
                if (TIPforS1 && TIPforS2)
                    return ip;
            }
            return new Point2d(double.NaN,double.NaN);
        }

        public static bool PFIPonlineseg(OffsetSegmentsCollection.offsetSegment seg, Point2d ip, OffsetSegmentsCollection.offsetSegment s2)
        {//先要用haslip判断
            Vector2d vsip = VectorCalculations.VectorOfTwoPoints(seg.seg.point, ip);
            Vector2d vse = VectorCalculations.VectorOfTwoPoints(seg.seg.point, seg.endOfSeg);
            Vector2d uvsip = VectorCalculations.unitVector(vsip);
            Vector2d uvse = VectorCalculations.unitVector(vse);
            double errorx=Math.Abs(uvsip.X-uvse.X);
            double errory=Math.Abs(uvsip.Y-uvse.Y);
            if (errorx <= SomeConstants.ERRORLIMIT && errory <= SomeConstants.ERRORLIMIT)
                return true;
            else
                return false;
        }

        public static bool HasIPBetweenTwoSegments(OffsetSegmentsCollection.offsetSegment seg1, OffsetSegmentsCollection.offsetSegment seg2)
        {
            if (seg1.seg.bulge == 0 && seg2.seg.bulge == 0)
            {
                LineEquation seg1_line = new LineEquation(seg1.seg, seg1.endOfSeg);
                LineEquation seg2_line = new LineEquation(seg2.seg, seg2.endOfSeg);

                if (seg1_line.HasIPWith(seg2_line)&&!seg1_line.IsOverlappingWith(seg2_line))
                {
                    Point2d[] ips = seg1_line.IPWith(seg2_line);
                    if (IsPointInSegNotisstartandend(ips[0], seg1) && IsPointInSegNotisstartandend(ips[0], seg2))
                    {
                        return true;
                    }
                    if (IsTheSamePoint(ips[0],seg1.seg.point))
                    {
                        if (IsTheSamePoint(ips[0],seg2.seg.point))
                        {
                            return true;
                        }
                        if (IsTheSamePoint(ips[0],seg2.endOfSeg))
                        {
                            return true;
                        }
                    }
                    if (IsTheSamePoint(ips[0],seg1.endOfSeg))
                    {
                        if (IsTheSamePoint(ips[0], seg2.seg.point))
                        {
                            return true;
                        }
                        if (IsTheSamePoint(ips[0], seg2.endOfSeg))
                        {
                            return true;
                        }
                    }
                }
            }
            if (seg1.seg.bulge != 0 && seg2.seg.bulge == 0)
            {
                CircleEquation seg1_circle = new CircleEquation(seg1.seg, seg1.endOfSeg);
                LineEquation seg2_line = new LineEquation(seg2.seg, seg2.endOfSeg);

                if (seg1_circle.HasIPWith(seg2_line))
                {
                    Point2d[] ips = seg1_circle.IPWith(seg2_line);

                    foreach (Point2d ip in ips)
                    {
                        if (IsPointInSegNotisstartandend(ip, seg1) && IsPointInSegNotisstartandend(ip, seg2))
                        {
                            return true;
                        }
                        if (IsTheSamePoint(ip, seg1.seg.point))
                        {
                            if (IsTheSamePoint(ip, seg2.seg.point))
                            {
                                return true;
                            }
                            if (IsTheSamePoint(ip, seg2.endOfSeg))
                            {
                                return true;
                            }
                        }
                        if (IsTheSamePoint(ip, seg1.endOfSeg))
                        {
                            if (IsTheSamePoint(ip, seg2.seg.point))
                            {
                                return true;
                            }
                            if (IsTheSamePoint(ip, seg2.endOfSeg))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            if (seg1.seg.bulge == 0 && seg2.seg.bulge != 0)
            {
                LineEquation seg1_line = new LineEquation(seg1.seg, seg1.endOfSeg);
                CircleEquation seg2_circle = new CircleEquation(seg2.seg, seg2.endOfSeg);

                if (seg1_line.HasIPWith(seg2_circle))
                {
                    Point2d[] ips = seg1_line.IPWith(seg2_circle);

                    foreach (Point2d ip in ips)
                    {
                        if (IsPointInSegNotisstartandend(ip, seg1) && IsPointInSegNotisstartandend(ip, seg2))
                        {
                            return true;
                        }
                        if (IsTheSamePoint(ip, seg1.seg.point))
                        {
                            if (IsTheSamePoint(ip, seg2.seg.point))
                            {
                                return true;
                            }
                            if (IsTheSamePoint(ip, seg2.endOfSeg))
                            {
                                return true;
                            }
                        }
                        if (IsTheSamePoint(ip, seg1.endOfSeg))
                        {
                            if (IsTheSamePoint(ip, seg2.seg.point))
                            {
                                return true;
                            }
                            if (IsTheSamePoint(ip, seg2.endOfSeg))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            if (seg1.seg.bulge != 0 && seg2.seg.bulge != 0)
            {
                CircleEquation seg1_circle = new CircleEquation(seg1.seg, seg1.endOfSeg);
                CircleEquation seg2_circle = new CircleEquation(seg2.seg, seg2.endOfSeg);

                if (seg1_circle.HasIPWith(seg2_circle)&&!seg1_circle.IsOverlappingWith(seg2_circle))
                {
                    Point2d[] ips = seg1_circle.IPWith(seg2_circle);

                    foreach (Point2d ip in ips)
                    {
                        if (IsPointInSegNotisstartandend(ip, seg1) && IsPointInSegNotisstartandend(ip, seg2))
                        {
                            return true;
                        }
                        if (IsTheSamePoint(ip, seg1.seg.point))
                        {
                            if (IsTheSamePoint(ip, seg2.seg.point))
                            {
                                return true;
                            }
                            if (IsTheSamePoint(ip, seg2.endOfSeg))
                            {
                                return true;
                            }
                        }
                        if (IsTheSamePoint(ip, seg1.endOfSeg))
                        {
                            if (IsTheSamePoint(ip, seg2.seg.point))
                            {
                                return true;
                            }
                            if (IsTheSamePoint(ip, seg2.endOfSeg))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public static List<Point2d> IPSBetweenTwoSegments(OffsetSegmentsCollection.offsetSegment seg1, OffsetSegmentsCollection.offsetSegment seg2)
        {
            List<Point2d> ipslist = new List<Point2d>();
            if (seg1.seg.bulge == 0 && seg2.seg.bulge == 0)
            {
                LineEquation seg1_line = new LineEquation(seg1.seg, seg1.endOfSeg);
                LineEquation seg2_line = new LineEquation(seg2.seg, seg2.endOfSeg);
                Point2d[] ips = seg1_line.IPWith(seg2_line);
                ipslist.Add(ips[0]);
                return ipslist;
            }
            if (seg1.seg.bulge != 0 && seg2.seg.bulge == 0)
            {
                CircleEquation seg1_circle = new CircleEquation(seg1.seg, seg1.endOfSeg);
                LineEquation seg2_line = new LineEquation(seg2.seg, seg2.endOfSeg);
                Point2d[] ips = seg1_circle.IPWith(seg2_line);

                    foreach (Point2d ip in ips)
                    {
                        if (IsPointInSegNotisstartandend(ip, seg1) && IsPointInSegNotisstartandend(ip, seg2))
                        {
                            ipslist.Add(ip);
                            continue;
                        }
                        if (IsTheSamePoint(ip,seg1.seg.point))
                        {
                            if (IsTheSamePoint(ip,seg2.seg.point))
                            {
                                ipslist.Add(ip);
                                continue;
                            }
                            if (IsTheSamePoint(ip,seg2.endOfSeg))
                            {
                                ipslist.Add(ip);
                                continue;
                            }
                        }
                        if (IsTheSamePoint(ip, seg1.endOfSeg))
                        {
                            if (IsTheSamePoint(ip, seg2.seg.point))
                            {
                                ipslist.Add(ip);
                                continue;
                            }
                            if (IsTheSamePoint(ip, seg2.endOfSeg))
                            {
                                ipslist.Add(ip);
                                continue;
                            }
                        }
                    }
            }
            if (seg1.seg.bulge == 0 && seg2.seg.bulge != 0)
            {
                LineEquation seg1_line = new LineEquation(seg1.seg, seg1.endOfSeg);
                CircleEquation seg2_circle = new CircleEquation(seg2.seg, seg2.endOfSeg);

                Point2d[] ips = seg1_line.IPWith(seg2_circle);

                    foreach (Point2d ip in ips)
                    {
                        if (IsPointInSegNotisstartandend(ip, seg1) && IsPointInSegNotisstartandend(ip, seg2))
                        {
                            ipslist.Add(ip);
                            continue;
                        }
                        if (IsTheSamePoint(ip, seg1.seg.point))
                        {
                            if (IsTheSamePoint(ip, seg2.seg.point))
                            {
                                ipslist.Add(ip);
                                continue;
                            }
                            if (IsTheSamePoint(ip, seg2.endOfSeg))
                            {
                                ipslist.Add(ip);
                                continue;
                            }
                        }
                        if (IsTheSamePoint(ip, seg1.endOfSeg))
                        {
                            if (IsTheSamePoint(ip, seg2.seg.point))
                            {
                                ipslist.Add(ip);
                                continue;
                            }
                            if (IsTheSamePoint(ip, seg2.endOfSeg))
                            {
                                ipslist.Add(ip);
                                continue;
                            }
                        }
                    }
            }
            if (seg1.seg.bulge != 0 && seg2.seg.bulge != 0)
            {
                CircleEquation seg1_circle = new CircleEquation(seg1.seg, seg1.endOfSeg);
                CircleEquation seg2_circle = new CircleEquation(seg2.seg, seg2.endOfSeg);

                if (seg1_circle.HasIPWith(seg2_circle) && !seg1_circle.IsOverlappingWith(seg2_circle))
                {
                    Point2d[] ips = seg1_circle.IPWith(seg2_circle);

                    foreach (Point2d ip in ips)
                    {
                        if (IsPointInSegNotisstartandend(ip, seg1) && IsPointInSegNotisstartandend(ip, seg2))
                        {
                            ipslist.Add(ip);
                            continue;
                        }
                        if (IsTheSamePoint(ip, seg1.seg.point))
                        {
                            if (IsTheSamePoint(ip, seg2.seg.point))
                            {
                                ipslist.Add(ip);
                                continue;
                            }
                            if (IsTheSamePoint(ip, seg2.endOfSeg))
                            {
                                ipslist.Add(ip);
                                continue;
                            }
                        }
                        if (IsTheSamePoint(ip, seg1.endOfSeg))
                        {
                            if (IsTheSamePoint(ip, seg2.seg.point))
                            {
                                ipslist.Add(ip);
                                continue;
                            }
                            if (IsTheSamePoint(ip, seg2.endOfSeg))
                            {
                                ipslist.Add(ip);
                                continue;
                            }
                        }
                    }
                }
            }
            return ipslist;
        }

        public static bool IsPointInSeg(Point2d point, OffsetSegmentsCollection.offsetSegment seg)
        {
            if (IsPointInSegNotisstartandend(point, seg.seg, seg.endOfSeg))
                return true;
            if (IsTheSamePoint(point, seg.seg.point))
                return true;
            if (IsTheSamePoint(point,seg.endOfSeg))
                return true;
            return false;
        }

        public static bool IsASegmengAPoint(OffsetSegmentsCollection.offsetSegment seg)
        {
            if (IsTheSamePoint(seg.seg.point, seg.endOfSeg))
                return true;
            else
                return false;
        }

        public static double LengthOfPlineSeg(List<OffsetSegmentsCollection.offsetSegment> pi)
        {
            double lengthofpline = 0;
            foreach (OffsetSegmentsCollection.offsetSegment o in pi)
            {
                if (o.seg.bulge == 0)
                {
                    Vector2d lv = VectorCalculations.VectorOfTwoPoints(o.seg.point, o.endOfSeg);
                    lengthofpline += VectorCalculations.NormOfVector(lv);
                }
                else
                {
                    CircleEquation aceq = new CircleEquation(o.seg, o.endOfSeg);
                    double r = aceq.r;
                    double angle = 4 * Math.Atan(o.seg.bulge);
                    lengthofpline += r * angle;
                }
            }
            return lengthofpline;
        }
    }
}
