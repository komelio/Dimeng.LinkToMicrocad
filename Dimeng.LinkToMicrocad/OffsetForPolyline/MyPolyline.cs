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
    public class MyPolyline
    {
        public List<SEG> segments = new List<SEG>();
        public bool IsClosed { get; set; }


        public void DealWithClosedSituation()
        {
            if (IsClosed)
            {
                segments.Add(segments[0]);
            }
        }

        public MyPolyline() { }

        public MyPolyline(List<SEG> segs)
        {
            segments = segs;
        }

        public MyPolyline(double[] xs, double[] ys, double[] bulges)
        {
            try
            {
                if (xs.Length != ys.Length || ys.Length != bulges.Length || xs.Length != bulges.Length)
                {
                    throw new ArgumentException("所输入的三个坐标的数组的长度不等!");
                }
                for (int i = 0; i < xs.Length; i++)
                {
                    Point2d p = new Point2d(xs[i], ys[i]);
                    segments.Add(new SEG(p, bulges[i]));
                }
            }
            catch
            {

            }
            
        }

        public SEG GetSegAt(int i)
        {
            return segments[i];
        }

        public bool IsPointOnPolyline(Point2d toAdd)
        {
            for (int i = 0; i < segments.Count - 1; i++)
            {
                if (PointWhere.IsPointInSegNotisstartandend(toAdd, segments[i], segments[i + 1].point))
                {
                    return true;
                }
            }
            return false;
        }

        public static MyPolyline Pretreatment(MyPolyline mypolyline)
        {
            int c = mypolyline.IsClosed ? mypolyline.segments.Count - 1 : mypolyline.segments.Count - 2;
            for (int i = 0; i < c; i++)
            {
                int k = i + 1;
                if (mypolyline.IsClosed && i==mypolyline.segments.Count-2)
                    k = 0;
                if (PointWhere.HasLIP(mypolyline.segments[i], mypolyline.segments[k].point, mypolyline.segments[k], mypolyline.segments[k+1].point))
                {
                    Point2d LIP = PointWhere.LIP(mypolyline.segments[i], mypolyline.segments[k].point, mypolyline.segments[k], mypolyline.segments[k+1].point);
                    Point2d PtoAdd;
                    SEG newSeg1, newSeg2;
                    SEG s1, s2;
                    mypolyline.segments[i].AddPoint(LIP, mypolyline.segments[k].point, out newSeg1, out newSeg2);
                    PtoAdd = newSeg2.midPointOfSeg(mypolyline.segments[k].point, mypolyline.segments[k].point);
                    mypolyline.segments[i].AddPoint(PtoAdd, mypolyline.segments[k].point, out s1, out s2);
                    mypolyline.segments.RemoveAt(i);
                    mypolyline.segments.Insert(i, s1);
                    mypolyline.segments.Insert(i+1, s2);
                    if (!(i == mypolyline.segments.Count - 2))
                        c++;
                }
            }

            return mypolyline;
        }

        public override string ToString()
        {
            string segs = "";
            string tmp = "";
            foreach (SEG seg in this.segments)
            {
                segs = seg.ToString();
                tmp = segs + tmp;
            }
            return tmp;
        }

        public bool IsLocalIntersection()
        {
            for (int i = 0; i < this.segments.Count - 2; i++)
            {
                if (PointWhere.HasLIP(this.segments[i], this.segments[i + 1].point, this.segments[i + 1], this.segments[i + 2].point))
                    return true;
            }
            return false;
        }

        public List<OffsetSegmentsCollection.offsetSegment> ConvertTooffsegsementType()
        {
            List<OffsetSegmentsCollection.offsetSegment> originalCurve = new List<OffsetSegmentsCollection.offsetSegment>();

            for (int i = 0; i < segments.Count; i++)
            {
                OffsetSegmentsCollection.offsetSegment toadd;
                if (i == segments.Count - 1)
                {
                    toadd.seg = segments[i];
                    toadd.endOfSeg = toadd.seg.point;
                    originalCurve.Add(toadd);
                    break;
                }
                else
                {
                    toadd.seg = segments[i];
                    toadd.endOfSeg = segments[i + 1].point;
                    originalCurve.Add(toadd);
                }
            }
            return originalCurve;
        }
    }
}
