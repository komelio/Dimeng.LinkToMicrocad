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
        public struct offsetSegment
        {
            public SEG seg;
            public Point2d endOfSeg;
        }

        public List<offsetSegment> plinetodrawing = new List<offsetSegment>();//用于画图
        public List<offsetSegment> originalCurve = new List<offsetSegment>();//预处理后的原多段线
        public List<offsetSegment> offsetsegments_collection_positive = new List<offsetSegment>();//各段的正向偏移
        public List<offsetSegment> offsetsegments_collection_negative = new List<offsetSegment>();//各段的反向偏移
        public List<offsetSegment> pline1 = new List<offsetSegment>();//正向的untrimmed offset curve
        public List<offsetSegment> pline2 = new List<offsetSegment>();//反向的untrimmed offset curve
        public MyPolyline mp;

        private Func<MyPolyline, double, List<OffsetSegmentsCollection.offsetSegment>> GetOffsetSegments;

        private Func<MyPolyline, double, List<OffsetSegmentsCollection.offsetSegment>> InitialDelegate()
        {
            this.GetOffsetSegments = GetPositiveOffsetSegments;
            this.GetOffsetSegments += GetNegativeOffsetSegments;
            return GetOffsetSegments;
        }

        public List<offsetSegment> GetPositiveOffsetSegments(MyPolyline myp, double offdist)
        {
            List<offsetSegment> poffsetsegments = new List<offsetSegment>();
            offsetSegment ofs;
            for (int i = 0; i < myp.segments.Count - 1; i++)
            {
                if (myp.segments[i].bulge == 0)
                {
                    ofs.seg = OffsetSegmentsCalculations.GetPositiveLineSegmentOffset(myp.segments[i].point, myp.segments[i + 1].point, offdist, out ofs.endOfSeg);
                    poffsetsegments.Add(ofs);
                }
                else
                {
                    ofs.seg = OffsetSegmentsCalculations.GetPositiveArcSegmentOffset(myp.segments[i], myp.segments[i + 1], offdist, out ofs.endOfSeg);
                    poffsetsegments.Add(ofs);
                }
            }
            ofs.seg = new SEG(poffsetsegments[poffsetsegments.Count-1].endOfSeg, 0);
            ofs.endOfSeg = ofs.seg.point;
            poffsetsegments.Add(ofs);
            return poffsetsegments;
        }

        public List<offsetSegment> GetNegativeOffsetSegments(MyPolyline myp, double offdist)
        {
            return GetPositiveOffsetSegments(myp, -offdist);
        }

        

        public OffsetSegmentsCollection(MyPolyline mypAfterPretreatment, double offsetdist)
        {
            GetOffsetSegments=InitialDelegate();
            Delegate[] MethodsToGetOffsetSegments=GetOffsetSegments.GetInvocationList();
            offsetsegments_collection_positive = ((Func<MyPolyline, double, List<OffsetSegmentsCollection.offsetSegment>>)MethodsToGetOffsetSegments[0])(mypAfterPretreatment, offsetdist);
            offsetsegments_collection_negative = ((Func<MyPolyline, double, List<OffsetSegmentsCollection.offsetSegment>>)MethodsToGetOffsetSegments[1])(mypAfterPretreatment, offsetdist);
            originalCurve = mypAfterPretreatment.ConvertTooffsegsementType();
            pline1utr = GetUntrimmedOffsetCurves(offsetsegments_collection_positive, mypAfterPretreatment);
            GetUntrimmedOffsetCurves(pline1utr);
            pline1 = plineutr;
            pline1utr = GetUntrimmedOffsetCurves(offsetsegments_collection_negative, mypAfterPretreatment);
            GetUntrimmedOffsetCurves(pline1utr);
            pline2 = plineutr;
            mp = mypAfterPretreatment;
            plinetodrawing = offsetsegments_collection_positive;
        }

        public override string ToString()
        {
            string s = "";
            foreach (offsetSegment o in plinetodrawing)
            {
                s += o.seg.ToString() + o.endOfSeg + "\n";
            }
            return s;
        }
    }
}
