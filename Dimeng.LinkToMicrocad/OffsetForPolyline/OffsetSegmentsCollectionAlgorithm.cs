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
        public List<SEG> pline1utr = new List<SEG>();//修剪和连接后各段的偏移
        public List<offsetSegment> plineutr = new List<offsetSegment>();//用来画修剪和连接后各段的偏移
        public void GetUntrimmedOffsetCurves(List<SEG> utr)
        {
            plineutr = new List<offsetSegment>();
            offsetSegment ofs;
            for (int i = 0; i < utr.Count - 1; i++)
            {
                ofs.seg = utr[i];
                ofs.endOfSeg = utr[i + 1].point;
                plineutr.Add(ofs);
            }
            ofs.seg = new SEG(plineutr[plineutr.Count - 1].endOfSeg, 0);
            ofs.endOfSeg = ofs.seg.point;
            plineutr.Add(ofs);
        }

        public List<SEG> GetUntrimmedOffsetCurves(List<offsetSegment> tountr, MyPolyline mp)
        {
            List<SEG> utr = new List<SEG>();
            utr.Add(tountr[0].seg);
            int tmpcount = mp.IsClosed ? tountr.Count - 1 : tountr.Count - 2;
            for (int i = 0; i < tmpcount; i++)
            {
                offsetSegment si_;
                offsetSegment sinext_;
                SEG tmp;
                if (i == tountr.Count - 2)
                {
                    si_.seg = utr[utr.Count - 1];
                    si_.endOfSeg = tountr[i].endOfSeg;
                    sinext_.seg = utr[0];
                    sinext_.endOfSeg = utr[1].point;
                    tmp = mp.segments[0];
                }
                else
                {
                    si_.seg = utr[utr.Count-1];
                    si_.endOfSeg = tountr[i].endOfSeg;
                    sinext_ = tountr[i + 1];
                    tmp = mp.segments[i + 1];
                }
                
                double sisegbulge = Math.Abs(si_.seg.bulge);
                double sinextsegbulge = Math.Abs(sinext_.seg.bulge);
                if (sisegbulge <= SomeConstants.ERRORLIMIT && sinextsegbulge <= SomeConstants.ERRORLIMIT)
                {//算法1
                    Algorithm1(si_, sinext_, utr);
                }
                else if (sisegbulge <= SomeConstants.ERRORLIMIT && sinextsegbulge > SomeConstants.ERRORLIMIT)
                {//算法2
                    Algorithm2(si_, sinext_, utr, tmp);
                }
                else if (sisegbulge > SomeConstants.ERRORLIMIT && sinextsegbulge <= SomeConstants.ERRORLIMIT)
                {//算法3
                    Algorithm3(si_, sinext_, utr, tmp);
                }
                else
                {//算法4
                    Algorithm4(si_, sinext_, utr, tmp);
                }
            }
            if (!mp.IsClosed)
                utr.Add(new SEG(tountr[tountr.Count - 1].seg.point, 0));
            else
            {
                utr.Add(utr[1]);
                utr.RemoveAt(0);
            }
            return utr;
        }

        private void Algorithm1(offsetSegment si_, offsetSegment sinext_, List<SEG> utr)//完全正确
        {
            LineEquation esi = new LineEquation(si_.seg, si_.endOfSeg);
            LineEquation esinext = new LineEquation(sinext_.seg, sinext_.endOfSeg);
            
            if (esi.IsOverlappingWith(esinext))
                utr.Add(new SEG(si_.endOfSeg, 0));
            else
            {
                Point2d[] ips = esi.IPWith(esinext);
                Point2d ip = ips[0];
                if (PointWhere.IsPointInSegNotisstartandend(ip, si_.seg, si_.endOfSeg))
                {
                    if (PointWhere.IsPointInSegNotisstartandend(ip, sinext_.seg, sinext_.endOfSeg))
                    {
                        utr.Add(new SEG(ip, 0));
                    }
                    else
                    {
                        utr.Add(new SEG(si_.endOfSeg, 0));
                        utr.Add(new SEG(sinext_.seg.point, 0));
                    }
                }
                else
                {
                    if (PointWhere.PFIPonlineseg(si_, ip, sinext_))
                    {
                        utr.Add(new SEG(ip, 0));
                    }
                    else
                    {
                        utr.Add(new SEG(si_.endOfSeg, 0));
                        utr.Add(new SEG(sinext_.seg.point, 0));
                    }
                }
            }
        }

        public void Algorithm2(offsetSegment si_line, offsetSegment sinext_arc, List<SEG> utr, SEG originalsegnext)
        {
            LineEquation esi = new LineEquation(si_line.seg, si_line.endOfSeg);
            CircleEquation esinext = new CircleEquation(sinext_arc.seg, sinext_arc.endOfSeg);
            Point2d c = new Point2d(esinext.a, esinext.b);
            
            if (esi.HasIPWith(esinext))
            {
                Point2d BIP;
                Point2d[] ips = esi.IPWith(esinext);
                double distance = VectorCalculations.NormOfVector(VectorCalculations.VectorOfTwoPoints(ips[0], originalsegnext.point));
                BIP = ips[0];
                if (ips.Length == 1||(ips.Length==2&&Math.Abs(ips[0].X-ips[1].X)<=SomeConstants.ERRORLIMIT&&Math.Abs(ips[0].Y-ips[1].Y)<=SomeConstants.ERRORLIMIT))
                {
                    Vector2d ctobip = VectorCalculations.VectorOfTwoPoints(c, BIP);
                    Vector2d ctosinextendp = VectorCalculations.VectorOfTwoPoints(c, sinext_arc.endOfSeg);
                    double angofctobip = VectorCalculations.GetAngleOfVector(ctobip);
                    double angofctosinextendp = VectorCalculations.GetAngleOfVector(ctosinextendp);
                    double angofnewseg, bulgeofnewseg;
                    if (sinext_arc.seg.bulge < 0)
                    {
                        if (angofctobip > angofctosinextendp)
                        {
                            angofnewseg = angofctobip - angofctosinextendp;
                        }
                        else
                        {
                            angofnewseg = angofctobip + 2 * Math.PI - angofctosinextendp;
                        }
                        bulgeofnewseg = -Math.Tan(angofnewseg / 4);
                        utr.Add(new SEG(BIP, bulgeofnewseg));
                        return;
                    }
                    else
                    {
                        if (angofctobip < angofctosinextendp)
                        {
                            angofnewseg = angofctosinextendp - angofctobip;
                        }
                        else
                        {
                            angofnewseg = angofctosinextendp + 2 * Math.PI - angofctobip;
                        }
                        bulgeofnewseg = Math.Tan(angofnewseg / 4);
                        utr.Add(new SEG(BIP, bulgeofnewseg));
                        return;
                    }
                }
                if (ips.Length > 1)
                {
                    foreach (Point2d ip in ips)
                    {
                        if (PointWhere.IsPointInSegNotisstartandend(ip, si_line.seg, si_line.endOfSeg) && PointWhere.IsPointInSegNotisstartandend(ip, sinext_arc.seg, sinext_arc.endOfSeg))
                        {
                            BIP = ip;
                            break;
                        }
                        if (distance > VectorCalculations.NormOfVector(VectorCalculations.VectorOfTwoPoints(ip, originalsegnext.point)))
                        {
                            BIP = ip;
                        }
                    }
                }

                if (PointWhere.IsPointInSegNotisstartandend(BIP, si_line.seg, si_line.endOfSeg) && PointWhere.IsPointInSegNotisstartandend(BIP, sinext_arc.seg, sinext_arc.endOfSeg))
                {
                    SEG seg1, segtoadd;
                    sinext_arc.seg.AddPoint(BIP, sinext_arc.endOfSeg, out seg1, out segtoadd);
                    utr.Add(segtoadd);
                    return;
                }
                else
                {
                    if ((Math.Abs(sinext_arc.seg.point.X - si_line.endOfSeg.X) <= SomeConstants.ERRORLIMIT && (Math.Abs(sinext_arc.seg.point.Y - si_line.endOfSeg.Y)) <= SomeConstants.ERRORLIMIT))
                    {
                        utr.Add(sinext_arc.seg);
                        return;
                    }
                    else
                    {
                        if (!PointWhere.IsPointInSegNotisstartandend(BIP, si_line.seg, si_line.endOfSeg) && !PointWhere.IsPointInSegNotisstartandend(BIP, sinext_arc.seg, sinext_arc.endOfSeg))
                        {
                            Point2d start = si_line.endOfSeg;
                            Point2d end = sinext_arc.seg.point;
                            Point2d center = originalsegnext.point;

                            Vector2d vecnormofline = VectorCalculations.VectorOfTwoPoints(originalsegnext.point, si_line.endOfSeg);
                            Vector2d vecparrofline = VectorCalculations.VectorOfTwoPoints(si_line.seg.point, si_line.endOfSeg);
                            Vector2d positivenormofline = VectorCalculations.PositiveNormalVector(vecparrofline);

                            Vector2d ctos = VectorCalculations.VectorOfTwoPoints(center, start);
                            Vector2d ctoe = VectorCalculations.VectorOfTwoPoints(center, end);
                            double angstart = VectorCalculations.GetAngleOfVector(ctos);
                            double angend = VectorCalculations.GetAngleOfVector(ctoe);
                            double angfromstarttoend = 0;
                            double bulge = 0;
                            
                            if (vecnormofline.X * positivenormofline.X > 0 || vecnormofline.Y * positivenormofline.Y > 0)
                            {
                                if (angstart > angend)
                                {
                                    angfromstarttoend = angstart - angend;
                                }
                                else
                                {
                                    angfromstarttoend = angstart + 2 * Math.PI - angend;
                                }
                                bulge = -Math.Tan(angfromstarttoend / 4);
                                utr.Add(new SEG(start, bulge));
                                utr.Add(sinext_arc.seg);
                                return;
                            }
                            else
                            {
                                if (angstart < angend)
                                {
                                    angfromstarttoend = angend - angstart;
                                }
                                else
                                {
                                    angfromstarttoend = angend + 2 * Math.PI - angstart;
                                }
                                bulge = Math.Tan(angfromstarttoend / 4);
                                utr.Add(new SEG(start, bulge));
                                utr.Add(sinext_arc.seg);
                                return;
                            }
                        }
                        else
                        {
                            utr.Add(new SEG(si_line.endOfSeg, 0));
                            utr.Add(sinext_arc.seg);
                            return;
                        }
                    }
                }
            }
            else
            {
                Point2d start = si_line.endOfSeg;
                Point2d end = sinext_arc.seg.point;
                Point2d center = originalsegnext.point;

                Vector2d vecnormofline = VectorCalculations.VectorOfTwoPoints(originalsegnext.point, si_line.endOfSeg);
                Vector2d vecparrofline = VectorCalculations.VectorOfTwoPoints(si_line.seg.point, si_line.endOfSeg);
                Vector2d positivenormofline = VectorCalculations.PositiveNormalVector(vecparrofline);

                Vector2d ctos = VectorCalculations.VectorOfTwoPoints(center, start);
                Vector2d ctoe = VectorCalculations.VectorOfTwoPoints(center, end);
                double angstart = VectorCalculations.GetAngleOfVector(ctos);
                double angend = VectorCalculations.GetAngleOfVector(ctoe);
                double angfromstarttoend = 0;
                double bulge = 0;
                if (vecnormofline.X * positivenormofline.X > 0 || vecnormofline.Y * positivenormofline.Y > 0)
                {
                    if (angstart > angend)
                    {
                        angfromstarttoend = angstart - angend;
                    }
                    else
                    {
                        angfromstarttoend = angstart + 2 * Math.PI - angend;
                    }
                    bulge = -Math.Tan(angfromstarttoend / 4);
                }
                else
                {
                    if (angstart < angend)
                    {
                        angfromstarttoend = angend - angstart;
                    }
                    else
                    {
                        angfromstarttoend = angend + 2 * Math.PI - angstart;
                    }
                    bulge = Math.Tan(angfromstarttoend / 4);
                }

                utr.Add(new SEG(start, bulge));
                utr.Add(sinext_arc.seg);
                return;
            }
        }

        public void Algorithm3(offsetSegment si_arc, offsetSegment sinext_line, List<SEG> utr, SEG originalsegnext)
        {
            CircleEquation esi = new CircleEquation(si_arc.seg, si_arc.endOfSeg);
            LineEquation esinext = new LineEquation(sinext_line.seg, sinext_line.endOfSeg);
            Point2d c = new Point2d(esi.a, esi.b);

            
            if (esi.HasIPWith(esinext))
            {
                Point2d[] ips = esi.IPWith(esinext);
                Point2d BIP;
                double distance = VectorCalculations.NormOfVector(VectorCalculations.VectorOfTwoPoints(ips[0], originalsegnext.point));
                BIP = ips[0];
                if (ips.Length == 1 || (ips.Length == 2 && Math.Abs(ips[0].X - ips[1].X) <= SomeConstants.ERRORLIMIT && Math.Abs(ips[0].Y - ips[1].Y) <= SomeConstants.ERRORLIMIT))
                {
                    Vector2d ctobip = VectorCalculations.VectorOfTwoPoints(c, BIP);
                    Vector2d ctoutrlaststartp = VectorCalculations.VectorOfTwoPoints(c, utr[utr.Count - 1].point);

                    double angofctobip = VectorCalculations.GetAngleOfVector(ctobip);
                    double angofctoutrlaststartp = VectorCalculations.GetAngleOfVector(ctoutrlaststartp);

                    double angofsegtoreplace, bulgeofsegtoreplace;

                    if (utr[utr.Count - 1].bulge < 0)
                    {
                        if (angofctoutrlaststartp > angofctobip)
                        {
                            angofsegtoreplace = angofctoutrlaststartp - angofctobip;
                        }
                        else
                        {
                            angofsegtoreplace = angofctoutrlaststartp + 2 * Math.PI - angofctobip;
                        }
                        bulgeofsegtoreplace = -Math.Tan(angofsegtoreplace / 4);
                        Point2d start = utr[utr.Count - 1].point;
                        utr.RemoveAt(utr.Count - 1);
                        utr.Add(new SEG(start, bulgeofsegtoreplace));
                        utr.Add(new SEG(BIP, 0));
                        return;
                    }
                    else
                    {
                        if (angofctobip > angofctoutrlaststartp)
                        {
                            angofsegtoreplace = angofctobip - angofctoutrlaststartp;
                        }
                        else
                        {
                            angofsegtoreplace = angofctobip + 2 * Math.PI - angofctoutrlaststartp;
                        }
                        bulgeofsegtoreplace = Math.Tan(angofsegtoreplace / 4);
                        Point2d start = utr[utr.Count - 1].point;
                        utr.RemoveAt(utr.Count - 1);
                        utr.Add(new SEG(start, bulgeofsegtoreplace));
                        utr.Add(new SEG(BIP, 0));
                        return;
                    }
                }
                if (ips.Length > 1)
                {
                    foreach (Point2d ip in ips)
                    {
                        if (PointWhere.IsPointInSegNotisstartandend(ip, si_arc.seg, si_arc.endOfSeg) && PointWhere.IsPointInSegNotisstartandend(ip, sinext_line.seg, sinext_line.endOfSeg))
                        {
                            BIP = ip;
                            break;
                        }
                        if (distance > VectorCalculations.NormOfVector(VectorCalculations.VectorOfTwoPoints(ip, originalsegnext.point)))
                        {
                            BIP = ip;
                        }
                    }
                }

                if (PointWhere.IsPointInSegNotisstartandend(BIP, si_arc.seg, si_arc.endOfSeg) && PointWhere.IsPointInSegNotisstartandend(BIP, sinext_line.seg, sinext_line.endOfSeg))
                {
                    SEG seg1, segtoadd;
                    utr[utr.Count-1].AddPoint(BIP, si_arc.endOfSeg, out seg1, out segtoadd);
                    utr.RemoveAt(utr.Count - 1);
                    utr.Add(seg1);
                    utr.Add(new SEG(BIP,0));
                    return;
                }
                else
                {
                    if ((Math.Abs(sinext_line.seg.point.X - si_arc.endOfSeg.X) <= SomeConstants.ERRORLIMIT && (Math.Abs(sinext_line.seg.point.Y - si_arc.endOfSeg.Y)) <= SomeConstants.ERRORLIMIT))
                    {
                        utr.Add(sinext_line.seg);
                        return;
                    }
                    else
                    {
                        if (!PointWhere.IsPointInSegNotisstartandend(BIP, si_arc.seg, si_arc.endOfSeg) && !PointWhere.IsPointInSegNotisstartandend(BIP, sinext_line.seg, sinext_line.endOfSeg))
                        {
                            Point2d start = si_arc.endOfSeg;
                            Point2d end = sinext_line.seg.point;
                            Point2d center = originalsegnext.point;

                            Vector2d vecnormofline = VectorCalculations.VectorOfTwoPoints(originalsegnext.point, sinext_line.seg.point);
                            Vector2d vecparrofline = VectorCalculations.VectorOfTwoPoints(sinext_line.seg.point, sinext_line.endOfSeg);
                            Vector2d positivenormofline = VectorCalculations.PositiveNormalVector(vecparrofline);

                            Vector2d ctos = VectorCalculations.VectorOfTwoPoints(center, start);
                            Vector2d ctoe = VectorCalculations.VectorOfTwoPoints(center, end);
                            double angstart = VectorCalculations.GetAngleOfVector(ctos);
                            double angend = VectorCalculations.GetAngleOfVector(ctoe);
                            double angfromstarttoend = 0;
                            double bulge = 0;
                            if (vecnormofline.X * positivenormofline.X > 0 || vecnormofline.Y * positivenormofline.Y > 0)
                            {
                                if (angstart > angend)
                                {
                                    angfromstarttoend = angstart - angend;
                                }
                                else
                                {
                                    angfromstarttoend = angstart + 2 * Math.PI - angend;
                                }
                                bulge = -Math.Tan(angfromstarttoend / 4);
                            }
                            else
                            {
                                if (angstart < angend)
                                {
                                    angfromstarttoend = angend - angstart;
                                }
                                else
                                {
                                    angfromstarttoend = angend + 2 * Math.PI - angstart;
                                }
                                bulge = Math.Tan(angfromstarttoend / 4);
                            }

                            utr.Add(new SEG(start, bulge));
                            utr.Add(sinext_line.seg);
                            return;
                        }
                        else
                        {
                            utr.Add(new SEG(si_arc.endOfSeg, 0));
                            utr.Add(sinext_line.seg);
                            return;
                        }
                    }
                }
            }
            else
            {
                Point2d start = si_arc.endOfSeg;
                Point2d end = sinext_line.seg.point;
                Point2d center = originalsegnext.point;

                Vector2d vecnormofline = VectorCalculations.VectorOfTwoPoints(originalsegnext.point, sinext_line.seg.point);
                Vector2d vecparrofline = VectorCalculations.VectorOfTwoPoints(sinext_line.seg.point, sinext_line.endOfSeg);
                Vector2d positivenormofline = VectorCalculations.PositiveNormalVector(vecparrofline);

                Vector2d ctos = VectorCalculations.VectorOfTwoPoints(center, start);
                Vector2d ctoe = VectorCalculations.VectorOfTwoPoints(center, end);
                double angstart = VectorCalculations.GetAngleOfVector(ctos);
                double angend = VectorCalculations.GetAngleOfVector(ctoe);
                double angfromstarttoend = 0;
                double bulge = 0;
                if (vecnormofline.X * positivenormofline.X > 0 || vecnormofline.Y * positivenormofline.Y > 0)
                {
                    if (angstart > angend)
                    {
                        angfromstarttoend = angstart - angend;
                    }
                    else
                    {
                        angfromstarttoend = angstart + 2 * Math.PI - angend;
                    }
                    bulge = -Math.Tan(angfromstarttoend / 4);
                }
                else
                {
                    if (angstart < angend)
                    {
                        angfromstarttoend = angend - angstart;
                    }
                    else
                    {
                        angfromstarttoend = angend + 2 * Math.PI - angstart;
                    }
                    bulge = Math.Tan(angfromstarttoend / 4);
                }

                utr.Add(new SEG(start, bulge));
                utr.Add(sinext_line.seg);
                return;
            }
        }

        public void Algorithm4(offsetSegment si_arc, offsetSegment sinext_arc, List<SEG> utr, SEG originalsegnext)
        {
            CircleEquation equationOfsiarc = new CircleEquation(si_arc.seg, si_arc.endOfSeg);
            CircleEquation equationOfsinextarc = new CircleEquation(sinext_arc.seg, sinext_arc.endOfSeg);
            
            Point2d c1 = new Point2d(equationOfsiarc.a, equationOfsiarc.b);
            Point2d c2 = new Point2d(equationOfsinextarc.a, equationOfsinextarc.b);
            Vector2d vec_c1toutrlaststartp = VectorCalculations.VectorOfTwoPoints(c1, utr[utr.Count-1].point);
            double angofvec_c1toutrlaststartp = VectorCalculations.GetAngleOfVector(vec_c1toutrlaststartp);
            Vector2d vec_c2tosinextendp = VectorCalculations.VectorOfTwoPoints(c2, sinext_arc.endOfSeg);
            double angofvec_c2tosinextendp = VectorCalculations.GetAngleOfVector(vec_c2tosinextendp);

            Vector2d vcc = VectorCalculations.VectorOfTwoPoints(c1, c2);
            double dist = VectorCalculations.NormOfVector(vcc);
            double r1 = equationOfsiarc.r;
            double r2 = equationOfsinextarc.r;
            double er1 = si_arc.endOfSeg.X - sinext_arc.seg.point.X;
            double er2 = si_arc.endOfSeg.Y - sinext_arc.seg.point.Y;

            if (Math.Abs(er1) <= SomeConstants.ERRORLIMIT && Math.Abs(er2) <= SomeConstants.ERRORLIMIT)
            {
                utr.Add(sinext_arc.seg);
                return;
            }

            if (equationOfsiarc.HasIPWith(equationOfsinextarc))
            {
                Point2d[] ips = equationOfsiarc.IPWith(equationOfsinextarc);
                Point2d BIP = ips[0];
                Vector2d ip0toorip = VectorCalculations.VectorOfTwoPoints(originalsegnext.point, ips[0]);
                double distance = VectorCalculations.NormOfVector(ip0toorip);
                foreach (Point2d ip in ips)
                {
                    if (PointWhere.IsPointInSegNotisstartandend(ip, si_arc.seg, si_arc.endOfSeg) && PointWhere.IsPointInSegNotisstartandend(ip, sinext_arc.seg, sinext_arc.endOfSeg))
                    {
                        BIP = ip;
                        SEG seg1, seg2;
                        utr[utr.Count-1].AddPoint(BIP, si_arc.endOfSeg, out seg1, out seg2);
                        utr.RemoveAt(utr.Count - 1);
                        utr.Add(seg1);
                        sinext_arc.seg.AddPoint(BIP, sinext_arc.endOfSeg, out seg1, out seg2);
                        utr.Add(seg2);
                         return;
                    }
                    if (distance > VectorCalculations.NormOfVector((VectorCalculations.VectorOfTwoPoints(ip, originalsegnext.point))))
                    {
                        BIP = ip;
                    }
                }

                if (!PointWhere.IsPointInSegNotisstartandend(BIP, si_arc.seg, si_arc.endOfSeg) && !PointWhere.IsPointInSegNotisstartandend(BIP, sinext_arc.seg, sinext_arc.endOfSeg))
                {
                    Vector2d c1tobip = VectorCalculations.VectorOfTwoPoints(c1, BIP);
                    Vector2d c2tobip = VectorCalculations.VectorOfTwoPoints(c2, BIP);

                    double angofc1tobip = VectorCalculations.GetAngleOfVector(c1tobip);
                    double angofc2tobip = VectorCalculations.GetAngleOfVector(c2tobip);

                    double angfromstarttoend1 = 0;
                    double angfromstarttoend2 = 0;
                    

                    double newbulge1, newbulge2;

                    if (utr[utr.Count-1].bulge > 0)
                    {
                        if (angofvec_c1toutrlaststartp < angofc1tobip)
                        {
                            angfromstarttoend1 = angofc1tobip - angofvec_c1toutrlaststartp;
                        }
                        else
                        {
                            angfromstarttoend1 = angofc1tobip + 2 * Math.PI - angofvec_c1toutrlaststartp;
                        }
                        newbulge1 = Math.Tan(angfromstarttoend1 / 4);
                        Point2d start = utr[utr.Count - 1].point;
                        utr.RemoveAt(utr.Count - 1);
                        utr.Add(new SEG(start, newbulge1));
                        if (sinext_arc.seg.bulge > 0)
                        {
                            if (angofvec_c2tosinextendp > angofc2tobip)
                            {
                                angfromstarttoend2 = angofvec_c2tosinextendp - angofc2tobip;
                            }
                            else
                            {
                                angfromstarttoend2 = angofvec_c2tosinextendp + 2 * Math.PI - angofc2tobip;
                            }
                            newbulge2 = Math.Tan(angfromstarttoend2 / 4);
                            utr.Add(new SEG(BIP, newbulge2));
                            return;
                        }
                        else
                        {
                            if (angofvec_c2tosinextendp < angofc2tobip)
                            {
                                angfromstarttoend2 = angofc2tobip - angofvec_c2tosinextendp;
                            }
                            else
                            {
                                angfromstarttoend2 = angofc2tobip + 2 * Math.PI - angofvec_c2tosinextendp;
                            }
                            newbulge2 = -Math.Tan(angfromstarttoend2 / 4);
                            utr.Add(new SEG(BIP, newbulge2));
                            return;
                        }
                    }
                    else
                    {
                        if (angofvec_c1toutrlaststartp > angofc1tobip)
                        {
                            angfromstarttoend1 = angofvec_c1toutrlaststartp - angofc1tobip;
                        }
                        else
                        {
                            angfromstarttoend1 = angofvec_c1toutrlaststartp + 2 * Math.PI - angofc1tobip;
                        }
                        Point2d start = utr[utr.Count - 1].point;
                        utr.RemoveAt(utr.Count - 1);
                        newbulge1 = -Math.Tan(angfromstarttoend1 / 4);
                        utr.Add(new SEG(start, newbulge1));
                        if (sinext_arc.seg.bulge > 0)
                        {
                            if (angofvec_c2tosinextendp > angofc2tobip)
                            {
                                angfromstarttoend2 = angofvec_c2tosinextendp - angofc2tobip;
                            }
                            else
                            {
                                angfromstarttoend2 = angofvec_c2tosinextendp + 2 * Math.PI - angofc2tobip;
                            }
                            newbulge2 = Math.Tan(angfromstarttoend2 / 4);
                            utr.Add(new SEG(BIP, newbulge2));
                            return;
                        }
                        else
                        {
                            if (angofvec_c2tosinextendp < angofc2tobip)
                            {
                                angfromstarttoend2 = angofc2tobip - angofvec_c2tosinextendp;
                            }
                            else
                            {
                                angfromstarttoend2 = angofc2tobip + 2 * Math.PI - angofvec_c2tosinextendp;
                            }
                            newbulge2 = -Math.Tan(angfromstarttoend2 / 4);
                            utr.Add(new SEG(BIP, newbulge2));
                            return;
                        }
                    }
                }
            }
            else
            {
                Vector2d c1tosiendp = VectorCalculations.VectorOfTwoPoints(c1, si_arc.endOfSeg);
                Vector2d siendptoorinextp = VectorCalculations.VectorOfTwoPoints(si_arc.endOfSeg, originalsegnext.point);

                Vector2d orinextptosiendp = VectorCalculations.VectorOfTwoPoints(originalsegnext.point, si_arc.endOfSeg);
                Vector2d orinextptosinextp = VectorCalculations.VectorOfTwoPoints(originalsegnext.point, sinext_arc.seg.point);

                double angoforinextptosiendp = VectorCalculations.GetAngleOfVector(orinextptosiendp);
                double angoforinextptosinextp = VectorCalculations.GetAngleOfVector(orinextptosinextp);
                double angofconnectarc;
                double bulgeofconnectarc;
                if (c1tosiendp.X * siendptoorinextp.X > 0 || c1tosiendp.Y * siendptoorinextp.Y > 0)
                {
                    if (si_arc.seg.bulge < 0)
                    {
                        if (angoforinextptosiendp < angoforinextptosinextp)
                        {
                            angofconnectarc = angoforinextptosinextp - angoforinextptosiendp;
                        }
                        else
                        {
                            angofconnectarc = angoforinextptosinextp + 2 * Math.PI - angoforinextptosiendp;
                        }
                        bulgeofconnectarc = Math.Tan(angofconnectarc / 4);
                        utr.Add(new SEG(si_arc.endOfSeg, bulgeofconnectarc));
                        utr.Add(sinext_arc.seg);
                        return;
                    }
                    if (angoforinextptosiendp > angoforinextptosinextp)
                    {
                        angofconnectarc = angoforinextptosiendp - angoforinextptosinextp;
                    }
                    else
                    {
                        angofconnectarc = angoforinextptosiendp + 2 * Math.PI - angoforinextptosinextp;
                    }
                    bulgeofconnectarc = -Math.Tan(angofconnectarc / 4);
                    utr.Add(new SEG(si_arc.endOfSeg, bulgeofconnectarc));
                    utr.Add(sinext_arc.seg);
                    return;
                }
                else
                {
                    if (angoforinextptosiendp < angoforinextptosinextp)
                    {
                        angofconnectarc = angoforinextptosiendp + 2 * Math.PI - angoforinextptosinextp;
                    }
                    else
                    {
                        angofconnectarc = angoforinextptosiendp - angoforinextptosinextp;
                    }
                    bulgeofconnectarc = -Math.Tan(angofconnectarc / 4);
                    utr.Add(new SEG(si_arc.endOfSeg, bulgeofconnectarc));
                    utr.Add(sinext_arc.seg);
                    return;
                }
            }
        }
    }
}
