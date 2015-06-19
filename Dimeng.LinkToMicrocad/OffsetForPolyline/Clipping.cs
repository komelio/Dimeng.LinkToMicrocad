using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Offset
{
    public partial class OffsetSegmentsCollection
    {
        public List<Point2d> GetSelfintersectionPoints(List<offsetSegment> pi)
        {
            List<Point2d> list_intersectionpoints = new List<Point2d>();
            for (int i = 0; i < pi.Count - 1; i++)
            {
                for (int j = 0; j < pi.Count - 1; j++)
                {
                    if (j == i)
                    {
                        continue;
                    }
                    if (PointWhere.HasIPBetweenTwoSegments(pi[i], pi[j]))
                    {
                        foreach (Point2d ip in PointWhere.IPSBetweenTwoSegments(pi[i], pi[j]))
                        {
                            if (j == i - 1 && PointWhere.IsTheSamePoint(ip,pi[i].seg.point))
                                continue;
                            if (j == i + 1 && PointWhere.IsTheSamePoint(ip,pi[i].endOfSeg))
                                continue;
                            bool iftoadd = true;
                            foreach (Point2d pinlist in list_intersectionpoints)
                            {
                                if (PointWhere.IsTheSamePoint(ip,pinlist))
                                {
                                    iftoadd = false;
                                    break;
                                }
                            }
                            if (iftoadd)
                            {
                                list_intersectionpoints.Add(ip);
                            }
                        }
                    }
                }
            }
            return list_intersectionpoints;
        }//求自交点,要求pi最后一个offsetSegment是点

        public List<Point2d> GetIntersectionPointsOfP1(List<offsetSegment> p1, List<offsetSegment> p2)
        {
            List<Point2d> list_intersectionpoints = new List<Point2d>();
            for (int i = 0; i < p1.Count - 1; i++)
            {
                for (int g = 0; g < p2.Count - 1; g++)
                {
                    if (PointWhere.HasIPBetweenTwoSegments(p1[i], p2[g]))
                    {
                        foreach (Point2d ip in PointWhere.IPSBetweenTwoSegments(p1[i], p2[g]))
                        {
                            bool iftoadd = true;
                            foreach (Point2d pinlist in list_intersectionpoints)
                            {
                                if (PointWhere.IsTheSamePoint(ip,pinlist))
                                {
                                    iftoadd = false;
                                    break;
                                }
                            }
                            if (iftoadd)
                            {
                                list_intersectionpoints.Add(ip);
                            }
                        }
                    }
                }
            }
            return list_intersectionpoints;
        }//求交点,要求p1和p2最后一个offsetSegment是点

        public List<List<Point2d>> SortIntersectionPointsOfPline(List<Point2d> ips, List<offsetSegment> pline)//给pline上的交点排序
        {
            List<List<Point2d>> ipsAfterSorted = new List<List<Point2d>>();

            for (int i = 0; i < pline.Count - 1; i++)
            {
                List<double> ipsdistance = new List<double>();
                List<Point2d> ipsOnSegI = new List<Point2d>();
                List<double> anglelist = new List<double>();

                for (int g = 0; g < ips.Count; g++)
                {
                    if (pline[i].seg.bulge != 0 && PointWhere.IsPointInSeg(ips[g], pline[i]))
                    {
                        double bulge=pline[i].seg.bulge;
                        CircleEquation cir = new CircleEquation(pline[i].seg, pline[i].endOfSeg);
                        Point2d circenter = new Point2d(cir.a, cir.b);
                        Vector2d ctoip = VectorCalculations.VectorOfTwoPoints(circenter, ips[g]);
                        Vector2d ctostart = VectorCalculations.VectorOfTwoPoints(circenter, pline[i].seg.point);
                        double angofctoip = VectorCalculations.GetAngleOfVector(ctoip);
                        double angofctostart = VectorCalculations.GetAngleOfVector(ctostart);
                        double angle = 0;
                        if (bulge > 0)
                        {
                            if (angofctostart < angofctoip)
                            {
                                angle = angofctoip - angofctostart;
                            }
                            else
                            {
                                angle = angofctoip + 2 * Math.PI - angofctostart;
                            }
                        }
                        else
                        {
                            if (angofctostart > angofctoip)
                            {
                                angle = angofctostart - angofctoip;
                            }
                            else
                            {
                                angle = angofctostart + 2 * Math.PI - angofctoip;
                            }
                        }
                        anglelist.Add(angle);
                        ipsOnSegI.Add(ips[g]);
                        continue;
                    }
                    if (PointWhere.IsPointInSeg(ips[g], pline[i]))
                    {
                        if (PointWhere.IsTheSamePoint(ips[g],pline[i].endOfSeg) && i != pline.Count - 2)
                            continue;
                        double distance = VectorCalculations.NormOfVector(VectorCalculations.VectorOfTwoPoints(ips[g], pline[i].seg.point));
                        ipsdistance.Add(distance);
                        ipsOnSegI.Add(ips[g]);
                    }
                }
                if (pline[i].seg.bulge != 0)
                {
                    for (int m = 0; m < anglelist.Count; m++)
                    {
                        bool change = false;
                        for (int n = 0; n < anglelist.Count - 1; n++)
                        {
                            if (anglelist[n] > anglelist[n + 1])
                            {
                                Point2d tmpPoint;
                                tmpPoint = ipsOnSegI[n];
                                ipsOnSegI[n] = ipsOnSegI[n + 1];
                                ipsOnSegI[n + 1] = tmpPoint;
                                double tmpangle;
                                tmpangle = anglelist[n];
                                anglelist[n] = anglelist[n + 1];
                                anglelist[n + 1] = tmpangle;
                                change = true;
                            }
                        }
                        if (!change)
                            break;
                    }
                    ipsAfterSorted.Add(ipsOnSegI);
                    continue;
                }
                for (int j = 0; j < ipsdistance.Count; j++)
                {
                    bool change = false;
                    for (int g = 0; g < ipsdistance.Count - 1; g++)
                    {

                        if (ipsdistance[g] > ipsdistance[g + 1])
                        {
                            Point2d tmp;
                            tmp = ipsOnSegI[g];
                            ipsOnSegI[g] = ipsOnSegI[g + 1];
                            ipsOnSegI[g + 1] = tmp;
                            double tmpdouble;
                            tmpdouble = ipsdistance[g];
                            ipsdistance[g] = ipsdistance[g + 1];
                            ipsdistance[g + 1] = tmpdouble;
                            change = true;
                        }
                    }
                    if (!change)
                        break;
                }   
                ipsAfterSorted.Add(ipsOnSegI);
            }
            return ipsAfterSorted;
        }

        List<List<offsetSegment>> tmpArray1 = new List<List<offsetSegment>>();

        List<List<offsetSegment>> tmpArray2 = new List<List<offsetSegment>>();

        List<List<offsetSegment>> finalResultArray = new List<List<offsetSegment>>();

        public bool IsThereAnyIPandSIPOnP1(List<offsetSegment> p1, List<offsetSegment> p2)
        {
            List<Point2d> SIPSOfP1 = GetSelfintersectionPoints(p1);
            List<Point2d> IPSOfP1 = GetIntersectionPointsOfP1(p1, p2);

            if (SIPSOfP1.Count == 0 && IPSOfP1.Count == 0)
                return false;
            else
                return true;
        }

        public bool IsThereAnyIPOnP1(List<offsetSegment> p1, List<offsetSegment> p2)
        {
            List<Point2d> IPSOfPline1 = GetIntersectionPointsOfP1(p1, p2);

            if (IPSOfPline1.Count == 0)
                return false;
            else
                return true;
        }

        public List<List<offsetSegment>> SplitPlineByIPSorted(List<List<Point2d>> ipsSorted, List<offsetSegment> pline)
        {
            List<List<offsetSegment>> splitresult = new List<List<offsetSegment>>();//最终按交点分段结果
            List<offsetSegment> splitsegi = new List<offsetSegment>();//其中的一段

            for (int i = 0; i < pline.Count - 1; i++)
            {
                offsetSegment tosplit = pline[i];//原untrimmedcurves中要分割的一段
                if (ipsSorted[i].Count == 0)
                {
                    splitsegi.Add(pline[i]);
                    if (i == pline.Count - 2)
                    {
                        splitsegi.Add(pline[pline.Count - 1]);
                    }
                    continue;
                }
                foreach (Point2d ip in ipsSorted[i])
                {
                    if (PointWhere.IsTheSamePoint(ip, tosplit.seg.point))
                    {
                        if (splitsegi.Count != 0 && PointWhere.IsASegmengAPoint(splitsegi[splitsegi.Count - 1]))
                        {
                            splitresult.Add(splitsegi);
                            splitsegi = new List<offsetSegment>();
                            offsetSegment os;
                            os.seg = new SEG(ip, 0);
                            os.endOfSeg = ip;
                            splitsegi.Add(os);
                            splitresult.Add(splitsegi);
                            splitsegi = new List<offsetSegment>();
                            continue;
                        }
                        else
                        {
                            if (splitsegi.Count != 0 && !PointWhere.IsASegmengAPoint(splitsegi[splitsegi.Count - 1]))
                            {
                                offsetSegment os;
                                os.seg = new SEG(ip, 0);
                                os.endOfSeg = ip;
                                splitsegi.Add(os);
                                splitresult.Add(splitsegi);
                                splitsegi = new List<offsetSegment>();
                                splitsegi.Add(os);
                                splitresult.Add(splitsegi);
                                splitsegi = new List<offsetSegment>();
                                continue;
                            }
                            else
                            {
                                offsetSegment os;
                                os.seg = new SEG(ip, 0);
                                os.endOfSeg = ip;
                                splitsegi.Add(os);
                                splitresult.Add(splitsegi);
                                splitsegi = new List<offsetSegment>();
                                continue;
                            }
                        }
                    }
                    if (PointWhere.IsTheSamePoint(ip, tosplit.endOfSeg))
                    {
                        splitsegi.Add(tosplit);
                        offsetSegment os;
                        os.seg = new SEG(ip, 0);
                        os.endOfSeg = ip;
                        splitsegi.Add(os);
                        splitresult.Add(splitsegi);
                        splitsegi = new List<offsetSegment>();
                        if (PointWhere.IsTheSamePoint(pline[pline.Count - 1].endOfSeg, ip))
                        {
                            return splitresult;
                        }
                        continue;
                    }
                    SEG seg1, seg2;
                    tosplit.seg.AddPoint(ip, tosplit.endOfSeg, out seg1, out seg2);
                    offsetSegment toadd;
                    toadd.seg = seg1;
                    toadd.endOfSeg = seg2.point;
                    splitsegi.Add(toadd);
                    offsetSegment endptoadd;
                    endptoadd.seg = new SEG(ip, 0);
                    endptoadd.endOfSeg = ip;
                    splitsegi.Add(endptoadd);
                    splitresult.Add(splitsegi);
                    splitsegi = new List<offsetSegment>();
                    tosplit.seg = seg2;
                    if (ipsSorted[i].IndexOf(ip) == (ipsSorted[i].Count - 1))
                    {
                        splitsegi.Add(tosplit);
                        if (i == pline.Count - 2)
                        {
                            splitsegi.Add(pline[pline.Count - 1]);
                            splitresult.Add(splitsegi);
                            return splitresult;
                        }
                        break;
                    }
                }
                if (i == pline.Count - 2)
                {
                    splitsegi.Add(pline[pline.Count - 1]);
                }
         }
         splitresult.Add(splitsegi);
         return splitresult;
        }

        public List<List<offsetSegment>> SplitPline1ByCircleOfIP(Point2d ip, List<offsetSegment> pi, double offsetdistance)
        {
            CircleEquation ipCircle = CircleEquation.GetCircleCenterForm(ip.X, ip.Y, offsetdistance);
            List<Point2d> ips = new List<Point2d>();//圆与segment的交点
           
            foreach (offsetSegment o in pi)
            {
                if (pi.IndexOf(o) == pi.Count - 1)
                    break;
                Point2d[] ipsarr;//圆与segment的交点,可能有重复
                switch (o.seg.bulge == 0)//求圆与segment的不重复交点
                {
                    case true:
                        LineEquation segline = new LineEquation(o.seg, o.endOfSeg);
                        if (segline.HasIPWith(ipCircle))
                        {
                            ipsarr = segline.IPWith(ipCircle);
                            foreach (Point2d p in ipsarr)
                            {
                                if (PointWhere.IsPointInSeg(p, o))
                                {
                                    bool toadd = true;
                                    foreach (Point2d ppp in ips)
                                    {
                                        if (PointWhere.IsTheSamePoint(ppp, p))
                                        {
                                            toadd = false;
                                            break;
                                        }
                                    }
                                    if (toadd)
                                        ips.Add(p);
                                }
                            }
                        }
                        break;
                    case false:
                        CircleEquation segCircle = new CircleEquation(o.seg, o.endOfSeg);
                        if (segCircle.HasIPWith(ipCircle))
                        {
                            ipsarr = segCircle.IPWith(ipCircle);
                            foreach (Point2d p in ipsarr)
                            {
                                if (PointWhere.IsPointInSeg(p, o))
                                {
                                    bool toadd2 = true;
                                    foreach (Point2d ppp in ips)
                                    {
                                        if (PointWhere.IsTheSamePoint(ppp, p))
                                        {
                                            toadd2 = false;
                                            break;
                                        }
                                    }
                                    if (toadd2)
                                        ips.Add(p);
                                }
                            }
                        }
                        break;
                }
            }
            if (ips.Count == 0)
                return null;
            double d1 = VectorCalculations.NormOfVector(VectorCalculations.VectorOfTwoPoints(ips[0], pi[0].seg.point));
            double d2 = VectorCalculations.NormOfVector(VectorCalculations.VectorOfTwoPoints(ips[0], pi[pi.Count - 1].endOfSeg));
            if (ips.Count == 1 && d1 > ipCircle.r && d2 > ipCircle.r)
            {
                return null;
            }
            List<List<Point2d>> ipssorted = SortIntersectionPointsOfPline(ips, pi);
            List<List<offsetSegment>> pisplitresult = SplitPlineByIPSorted(ipssorted, pi);
            if (ips.Count == 2)
            {
                pisplitresult.RemoveAt(1);
            }
            else
            {
                if (ips.Count == 1)
                {
                    double dis1 = VectorCalculations.NormOfVector(VectorCalculations.VectorOfTwoPoints(ip, pisplitresult[0][0].seg.point));
                    double dis2 = VectorCalculations.NormOfVector(VectorCalculations.VectorOfTwoPoints(ip, pisplitresult[pisplitresult.Count - 1][pisplitresult[pisplitresult.Count - 1].Count - 1].endOfSeg));
                    if (dis1 < dis2)
                    {
                        pisplitresult.RemoveAt(0);
                    }
                    else
                    {
                        pisplitresult.RemoveAt(1);
                    }
                }
            }
            List<List<offsetSegment>> pisplitbyipcircleresult = pisplitresult;
            return pisplitbyipcircleresult;
        }

        public List<T> ConvertFrom<T>(List<List<T>> llos)
        {
            List<T> los = new List<T>();

            foreach (List<T> lo in llos)
            {
                foreach (T o in lo)
                {
                    los.Add(o);
                }
            }
            return los;
        }

        public void GetOffsetFinalResult(List<offsetSegment> pline_1, List<offsetSegment> pline_2, double d)
        {
            List<Point2d> allPoints = new List<Point2d>();
            plinetodrawing = new List<offsetSegment>();
            tmpArray1 = new List<List<offsetSegment>>();
            tmpArray2 = new List<List<offsetSegment>>();
            List<offsetSegment> tmppicol = new List<offsetSegment>();
            finalResultArray = new List<List<offsetSegment>>();

            //步骤1b的情况一
            List<Point2d> intersectionPoints = GetIntersectionPointsOfP1(pline_1, pline_2);
            List<Point2d> SelfIntersectionPoints = GetSelfintersectionPoints(pline_1);
            if (intersectionPoints.Count == 0 && SelfIntersectionPoints.Count == 0)
            {
                tmpArray1.Add(pline_1);
            }
            else
            {//情况二
                
                allPoints.AddRange(intersectionPoints);
                allPoints.AddRange(SelfIntersectionPoints);
                if (mp.IsClosed)
                {
                    List<int> todelete = new List<int>();
                    for (int i = 0; i < allPoints.Count; i++)
                    {
                        if (PointWhere.IsTheSamePoint(allPoints[i], pline_1[0].seg.point))
                            todelete.Add(i);
                    }
                    foreach (int j in todelete)
                    {
                        allPoints.RemoveAt(j);
                    }
                }
                List<List<Point2d>> ipsSorted = SortIntersectionPointsOfPline(allPoints, pline_1);
                
                List<List<offsetSegment>> piCollection = SplitPlineByIPSorted(ipsSorted, pline_1);
                tmppicol = ConvertFrom(piCollection);
                foreach (List<offsetSegment> pi in piCollection)
                {
                    if (pi.Count == 1 && PointWhere.IsASegmengAPoint(pi[0]))
                        continue;
                    List<Point2d> ipwithOriginalCurve = GetIntersectionPointsOfP1(pi, originalCurve);
                    if (ipwithOriginalCurve.Count == 0)
                    {
                        tmpArray1.Add(pi);
                        continue;
                    }
                    continue;
                }

            }

            //最后把集合中的集合合并为一个大线条
            List<offsetSegment> tmpPline = ConvertFrom(tmpArray1);
            for (int h = 0; h < tmpPline.Count - 1; h++)
            {
                if (PointWhere.IsASegmengAPoint(tmpPline[h]))
                {
                    tmpPline.Remove(tmpPline[h]);
                    h--;
                    continue;
                }
            }
            plinetodrawing = tmpPline;
            //plinetodrawing = pline1;
            //Document doc = Application.DocumentManager.MdiActiveDocument;
            //Editor ed = doc.Editor;
            //foreach (Point2d p in SelfIntersectionPoints)
            //{
            //    ed.WriteMessage("点"+p.ToString());
            //}
        }
    }
}
