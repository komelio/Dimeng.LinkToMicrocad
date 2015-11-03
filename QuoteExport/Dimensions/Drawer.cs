using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace QuoteExport.Dimensions
{
    public class Drawer
    {
        Graphics g;
        Part part;
        float ratio;
        float drawingWidth = 600;
        float drawingHeight = 380;
        float gap = 100;
        float vdrillNumberSize = 13;
        Pen borderPen = new Pen(Color.DarkGreen);
        Pen hdrillPen = new Pen(Color.Blue) { DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot };
        Pen vdrillPen = new Pen(Color.Black);
        Pen wipePen = new Pen(Color.White);
        Pen routePen = new Pen(Color.Red);

        bool isMirrorPart = false;

        List<DrillInfo> vdrillInfos = new List<DrillInfo>();
        List<DrillInfo> hdrillInfos = new List<DrillInfo>();

        public Drawer(Graphics graphics, Part part, float canvasWidth, float canvasHeight)
        {
            this.g = graphics;
            this.part = part;

            this.drawingWidth = canvasWidth - gap * 3;
            this.drawingHeight = Convert.ToSingle(canvasHeight - gap * 2);

            float ratio1 = Convert.ToSingle(drawingHeight / part.Border.PanelWidth);
            float ratio2 = Convert.ToSingle(drawingWidth / part.Border.PanelLength);
            ratio = ratio1 < ratio2 ? ratio1 : ratio2;

            if ((part.Border.Face5Only && part.Border.MachinePoint.EndsWith("M"))
                || (part.Border.Face6Only && !part.Border.MachinePoint.EndsWith("M"))
                || (part.Border.DoubleFace5 && part.Border.MachinePoint.EndsWith("M"))
                || (part.Border.DoubleFace6 && !part.Border.MachinePoint.EndsWith("M")))
            {
                this.isMirrorPart = true;
            }

            getVDrillInfos();
            getHDrillInfos();
        }

        private void getHDrillInfos()
        {
            this.hdrillInfos.Clear();

            foreach (var hd in part.Hdrillings)
            {
                var hdi = this.hdrillInfos.Find(it => it.Diameter == hd.HDrillDiameter && it.Depth == getHdrillDepth(hd));
                if (hdi == null)
                {
                    this.hdrillInfos.Add(new DrillInfo(0, hd.HDrillDiameter, getHdrillDepth(hd)));
                }
            }
        }

        private float getHdrillDepth(HdrillSeq hd)
        {
            if (hd.CurrentFace == "1" || hd.CurrentFace == "2")
            {
                return hd.HDrillY;
            }
            else
            {
                return (hd.HDrillX > part.Border.PanelLength / 2) ? (part.Border.PanelLength - hd.HDrillX) : hd.HDrillX;
            }
        }

        private void getVDrillInfos()
        {
            vdrillInfos.Clear();

            foreach (var vd in part.Vdrillings)
            {
                var vdi = this.vdrillInfos.Find(it => it.Diameter == vd.VDrillDiameter && it.Depth == vd.VDrillZ);
                if (vdi == null)
                {
                    vdrillInfos.Add(new DrillInfo(vdrillInfos.Count + 1, vd.VDrillDiameter, vd.VDrillZ));
                }
            }
        }

        ~Drawer()
        {
            borderPen.Dispose();
            hdrillPen.Dispose();
            vdrillPen.Dispose();
            wipePen.Dispose();
        }

        public void Draw()
        {
            g.DrawRectangle(borderPen, gap, gap,
                Convert.ToSingle(part.Border.PanelLength) * ratio,
                Convert.ToSingle(part.Border.PanelWidth) * ratio);

            foreach (var vd in part.Vdrillings)
            {
                drawVdrill(vd);
            }
            foreach (var hd in part.Hdrillings)
            {
                drawHdrill(hd);
            }
            foreach (var r in part.Routes)
            {
                drawRoute(r);
            }

            drawVdrillInfos();
            drawHdrillInfos();

            List<float> leftList = getLeftList();
            List<float> topList = getTopList();

            drawLeftDimensions(leftList);
            drawTopDimensions(topList);

            drawPartBordeDimension();
        }

        private void drawRoute(RouteSeq r)
        {
            if (r.RouteSeqs.Count == 0)
                throw new Exception("铣型分段数必须大于等于1");

            var list = new List<RouteOneSeq>(r.RouteSeqs);
            list.Insert(0, r.SetMillSeq);

            if (!isMirrorPart)
            {
                for (int i = 1; i < list.Count; i++)
                {
                    float x1, y1;
                    x1 = (i == 1) ? list[0].RouteSetMillX : list[i - 1].RouteX;
                    y1 = (i == 1) ? list[0].RouteSetMillY : list[i - 1].RouteY;
                    if (i == 1)
                    {
                        this.g.DrawEllipse(this.routePen,
                            this.gap + list[0].RouteStartOffsetX * this.ratio - list[0].RouteDiameter / 2 * this.ratio,
                            this.gap + list[0].RouteStartOffsetY * this.ratio - list[0].RouteDiameter / 2 * this.ratio,
                            list[0].RouteDiameter * this.ratio,
                            list[0].RouteDiameter * this.ratio);
                    }

                    if (list[i].RouteBulge == 0)
                    {

                        g.DrawLine(borderPen,
                            gap + x1 * ratio,
                            gap + y1 * ratio,
                            gap + list[i].RouteX * ratio,
                            gap + list[i].RouteY * ratio);
                    }
                    else
                    {
                        float angle1 = GetAngle(x1, y1, list[i].RouteCenterX, list[i].RouteCenterY);
                        float angle2 = GetAngle(list[i].RouteX, list[i].RouteY, list[i].RouteCenterX, list[i].RouteCenterY);

                        float sweepAngle;
                        if (list[i].RouteBulge > 0)
                            sweepAngle = angle1 > angle2 ?
                                         angle2 - angle1 :
                                         angle2 - angle1 - 360;
                        else
                            sweepAngle = angle1 < angle2 ?
                                         angle2 - angle1 :
                                         360 - angle1 + angle2;

                        g.DrawArc(borderPen,
                            gap + (list[i].RouteCenterX - list[i].RouteRadius) * ratio,
                            gap + (list[i].RouteCenterY - list[i].RouteRadius) * ratio,
                            list[i].RouteRadius * ratio * 2,
                            list[i].RouteRadius * ratio * 2,
                            angle1,
                            //5);
                            sweepAngle);

                    }
                }
            }
            else
            {
                for (int i = 1; i < list.Count; i++)
                {
                    float x1, y1;
                    x1 = (i == 1) ? list[0].RouteSetMillX : list[i - 1].RouteX;
                    y1 = (i == 1) ? list[0].RouteSetMillY : list[i - 1].RouteY;

                    if (i == 1)
                    {
                        this.g.DrawEllipse(this.routePen,
                            this.gap + (this.part.Border.PanelLength - list[0].RouteStartOffsetX) * this.ratio - list[0].RouteDiameter / 2 * this.ratio,
                            this.gap + (list[0].RouteStartOffsetY) * this.ratio - list[0].RouteDiameter / 2 * this.ratio,
                            list[0].RouteDiameter * this.ratio,
                            list[0].RouteDiameter * this.ratio);
                    }

                    if (list[i - 1].RouteBulge == 0)
                    {

                        g.DrawLine(borderPen,
                            gap + (part.Border.PanelLength - x1) * ratio,
                            gap + y1 * ratio,
                            gap + (part.Border.PanelLength - list[i].RouteX) * ratio,
                            gap + list[i].RouteY * ratio);
                    }
                    else
                    {
                        float angle1 = GetAngle(x1, y1, list[i].RouteCenterX, list[i].RouteCenterY);
                        float angle2 = GetAngle(list[i].RouteX, list[i].RouteY, list[i].RouteCenterX, list[i].RouteCenterY);

                        float sweepAngle;
                        if (list[i].RouteBulge > 0)
                            sweepAngle = angle1 > angle2 ?
                                         angle2 - angle1 :
                                         angle2 - angle1 - 360;
                        else
                            sweepAngle = angle1 < angle2 ?
                                         angle2 - angle1 :
                                         360 - angle1 + angle2;

                        g.DrawArc(borderPen,
                            gap + (part.Border.PanelLength - list[i].RouteCenterX - list[i].RouteRadius) * ratio,
                            gap + (list[i].RouteCenterY - list[i].RouteRadius) * ratio,
                            list[i].RouteRadius * ratio * 2,
                            list[i].RouteRadius * ratio * 2,
                            180 - angle1,
                            //5);
                            -sweepAngle);

                    }
                }
            }
        }
        private float GetAngle(float x, float y, float centerX, float centerY)
        {
            float angle;
            float tempY = -y + centerY;
            float tempX = x - centerX;
            if (tempY > 0 || (tempY == 0 && tempX < 0))
                angle = 360 - Convert.ToSingle(Math.Atan2(tempY, tempX) / Math.PI * 180);
            else
                angle = -Convert.ToSingle(Math.Atan2(tempY, tempX) / Math.PI * 180);
            return angle;
        }

        private List<float> getTopList()
        {
            List<float> list = new List<float>();
            foreach (var v in part.Vdrillings)
            {
                if (!this.isMirrorPart)
                {
                    list.Add(v.VDrillX);
                }
                else
                {
                    list.Add(part.Border.PanelLength - v.VDrillX);
                }
            }
            foreach (var h in part.Hdrillings)
            {
                if (h.CurrentFace == "1" || h.CurrentFace == "2")
                {
                    if (!this.isMirrorPart)
                    {
                        list.Add(h.HDrillX);
                    }
                    else
                    {
                        list.Add(part.Border.PanelLength - h.HDrillX);
                    }
                }
            }

            return list;
        }

        private List<float> getLeftList()
        {
            List<float> list = new List<float>();
            foreach (var v in part.Vdrillings)
            {
                list.Add(v.VDrillY);
            }
            foreach (var h in part.Hdrillings)
            {
                if (h.CurrentFace == "3" || h.CurrentFace == "4")
                {
                    list.Add(h.HDrillY);
                }
            }

            return list;
        }

        private void drawLeftDimensions(List<float> values)
        {
            values = values.Distinct().OrderBy(it => it).ToList();
            foreach (float x in values)
            {
                int index = values.IndexOf(x);

                g.DrawLine(borderPen, gap - 10, gap + x * ratio, gap - 20, gap + x * ratio);


                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;


                float valueBefore = (index > 0) ? values[index - 1] : 0;
                g.DrawString((x - valueBefore).ToString(),
                    new Font("Arial", 10),
                    new SolidBrush(Color.DarkGreen),
                    gap - 20,
                    gap + (valueBefore + (x - valueBefore) / 2) * ratio,
                    sf);


                if (index == values.Count - 1)
                {
                    g.DrawString((part.Border.PanelWidth - values[index]).ToString(),
                        new Font("Arial", 10),
                        new SolidBrush(Color.DarkGreen),
                        gap - 20,
                        gap + (values[index] + (part.Border.PanelWidth - values[index]) / 2) * ratio,
                        sf);
                }
            }
        }

        private void drawHdrillInfos()
        {
            if (this.hdrillInfos.Count == 0)
                return;

            g.DrawString("水平孔：",
                    new Font("Arial", 10),
                    new SolidBrush(Color.Red),
                    gap + part.Border.PanelLength * ratio + 30,
                    gap + (this.vdrillInfos.Count + 1) * 20 + 30
               );

            foreach (var hdi in this.hdrillInfos)
            {
                int index = hdrillInfos.IndexOf(hdi) + 1;
                g.DrawString(string.Format("φ{0,-3} D {1,-3}", hdi.Diameter, hdi.Depth),
                    new Font("Arial", 10),
                    new SolidBrush(Color.Red),
                    gap + part.Border.PanelLength * ratio + 30,
                    gap + (this.vdrillInfos.Count + 1) * 20 + 30 + index * 20
               );
            }
        }

        private void drawPartBordeDimension()
        {
            g.DrawLine(borderPen, gap, gap - 10, gap, gap - 70);
            g.DrawLine(borderPen, gap + part.Border.PanelLength * ratio, gap - 10, gap + part.Border.PanelLength * ratio, gap - 70);
            g.DrawLine(borderPen, gap, gap - 58, gap + part.Border.PanelLength * ratio / 2 - 20, gap - 58);
            g.DrawLine(borderPen, gap + part.Border.PanelLength * ratio / 2 + 20, gap - 58, gap + part.Border.PanelLength * ratio, gap - 58);

            g.DrawString(part.Border.PanelLength.ToString(),
                    new Font("Arial", 10),
                    new SolidBrush(Color.DarkGreen),
                    gap + part.Border.PanelLength * ratio / 2,
                    gap - 60,
                    new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center }
               );

            g.DrawLine(borderPen, gap - 10, gap, gap - 70, gap);
            g.DrawLine(borderPen, gap - 10, gap + part.Border.PanelWidth * ratio, gap - 70, gap + part.Border.PanelWidth * ratio);
            g.DrawLine(borderPen, gap - 62, gap, gap - 62, gap + part.Border.PanelWidth / 2 * ratio - 20);
            g.DrawLine(borderPen, gap - 62, gap + part.Border.PanelWidth * ratio, gap - 62, gap + part.Border.PanelWidth / 2 * ratio + 20);

            g.DrawString(part.Border.PanelWidth.ToString(),
                    new Font("Arial", 10),
                    new SolidBrush(Color.DarkGreen),
                    gap - 70,
                    gap + part.Border.PanelWidth * ratio / 2,
                    new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center }
               );
        }

        private void drawVdrillInfos()
        {
            if (this.vdrillInfos.Count == 0)
                return;

            float gapX = 30;
            float x = gap + part.Border.PanelLength * ratio + gapX;
            float y = gap;

            g.DrawString("垂直孔：",
                    new Font("Arial", 10),
                    new SolidBrush(Color.Red),
                    x,
                    y);

            foreach (var vdi in this.vdrillInfos)
            {
                g.DrawString(string.Format("{0} : φ{1,-4} D{2,-3}", vdi.Index, vdi.Diameter, vdi.Depth),
                    new Font("Arial", 10),
                    new SolidBrush(Color.Red),
                    x,
                    y + (vdrillInfos.IndexOf(vdi) + 1) * 20);
            }
        }

        private void drawTopDimensions(List<float> values)
        {
            values = values.Distinct().OrderBy(it => it).ToList();
            foreach (float x in values)
            {
                int index = values.IndexOf(x);

                g.DrawLine(borderPen, gap + x * ratio, gap - 10, gap + x * ratio, gap - 20);


                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;


                float valueBefore = (index > 0) ? values[index - 1] : 0;
                g.DrawString((x - valueBefore).ToString(),
                    new Font("Arial", 10),
                    new SolidBrush(Color.DarkGreen),
                    gap + (valueBefore + (x - valueBefore) / 2) * ratio,
                    gap - 20,
                    sf);


                if (index == values.Count - 1)
                {
                    g.DrawString((part.Border.PanelLength - values[index]).ToString(),
                        new Font("Arial", 10),
                        new SolidBrush(Color.DarkGreen),
                        gap + (values[index] + (part.Border.PanelLength - values[index]) / 2) * ratio,
                        gap - 20,
                        sf);
                }
            }
        }

        private void drawVdrill(VdrillSeq vd)
        {
            if (!this.isMirrorPart)
            {
                g.DrawEllipse(vdrillPen,
                    gap + (vd.VDrillX - vd.VDrillDiameter / 2) * ratio,
                    gap + (vd.VDrillY - vd.VDrillDiameter / 2) * ratio,
                    vd.VDrillDiameter * ratio,
                    vd.VDrillDiameter * ratio);

                string index = vdrillInfos.Find(it => it.Diameter == vd.VDrillDiameter && it.Depth == vd.VDrillZ).Index.ToString();
                g.DrawString(index, new Font("Arial", vdrillNumberSize), new SolidBrush(Color.Red),
                    gap + (vd.VDrillX + vd.VDrillDiameter) * ratio,
                    gap + (vd.VDrillY + vd.VDrillDiameter) * ratio);
            }
            else
            {
                g.DrawEllipse(vdrillPen,
                    gap + (part.Border.PanelLength - vd.VDrillX - vd.VDrillDiameter / 2) * ratio,
                    gap + (vd.VDrillY - vd.VDrillDiameter / 2) * ratio,
                    vd.VDrillDiameter * ratio,
                    vd.VDrillDiameter * ratio);

                string index = vdrillInfos.Find(it => it.Diameter == vd.VDrillDiameter && it.Depth == vd.VDrillZ).Index.ToString();
                g.DrawString(index, new Font("Arial", vdrillNumberSize), new SolidBrush(Color.Red),
                    gap + (part.Border.PanelLength - vd.VDrillX + vd.VDrillDiameter) * ratio,
                    gap + (vd.VDrillY + vd.VDrillDiameter) * ratio);
            }
        }

        private void drawHdrill(HdrillSeq hd)
        {
            float x, y, l, w;
            if (hd.CurrentFace == "1")
            {
                x = gap + (((this.isMirrorPart) ? part.Border.PanelLength - hd.HDrillX : hd.HDrillX) - hd.HDrillDiameter / 2) * ratio;
                y = gap;
                l = hd.HDrillDiameter * ratio;
                w = hd.HDrillY * ratio;
            }
            else if (hd.CurrentFace == "2")
            {
                x = gap + (((this.isMirrorPart) ? part.Border.PanelLength - hd.HDrillX : hd.HDrillX) - hd.HDrillDiameter / 2) * ratio;
                y = gap + (part.Border.PanelWidth - hd.HDrillY) * ratio;
                l = hd.HDrillDiameter * ratio;
                w = hd.HDrillY * ratio;
            }
            else if ((hd.CurrentFace == "3" && !this.isMirrorPart) ||
                (hd.CurrentFace == "4" && this.isMirrorPart))
            {
                float hdx = (hd.HDrillX > part.Border.PanelLength / 2) ? (part.Border.PanelLength - hd.HDrillX) : hd.HDrillX;
                x = gap;
                y = gap + (hd.HDrillY - hd.HDrillDiameter / 2) * ratio;
                l = hdx * ratio;
                w = hd.HDrillDiameter * ratio;
            }
            else
            {
                float hdx = (hd.HDrillX > part.Border.PanelLength / 2) ? (part.Border.PanelLength - hd.HDrillX) : hd.HDrillX;
                x = gap + (part.Border.PanelLength - hdx) * ratio;
                y = gap + (hd.HDrillY - hd.HDrillDiameter / 2) * ratio;
                l = hdx * ratio;
                w = hd.HDrillDiameter * ratio;
            }

            g.DrawRectangle(hdrillPen, x, y, l, w);
        }
    }
}
