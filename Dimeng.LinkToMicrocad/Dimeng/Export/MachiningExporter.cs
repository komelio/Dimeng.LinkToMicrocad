using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dimeng.WoodEngine.Entities;
using Dimeng.WoodEngine.Entities.Machines.Tools;
using Dimeng.WoodEngine.Entities.Machinings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dimeng.LinkToMicrocad
{
    public class MachiningExporter
    {
        PartMachiningType MachiningType;
        Part part;
        double panelLength;
        double panelWidth;
        double panelCutLength;
        double panelCutWidth;
        string path;
        bool hasMachining;

        List<HDrilling> HDrillings = new List<HDrilling>();
        List<VDrilling> VDrillings = new List<VDrilling>();
        List<VDrilling> VDrillingsFace6 = new List<VDrilling>();
        List<Routing> Routings = new List<Routing>();
        List<Routing> RoutingsFace6 = new List<Routing>();
        List<Sawing> Sawings = new List<Sawing>();
        List<Sawing> SawingsFace6 = new List<Sawing>();

        public MachiningExporter(Part part, string outputpath)
        {
            this.part = part;
            this.path = outputpath;

            #region 根据旋转，设定尺寸
            if (part.MachinePoint.IsRotated)
            {
                panelLength = part.Width;
                panelWidth = part.Length;
                panelCutLength = part.CutWidth;
                panelCutWidth = part.CutLength;
            }
            else
            {
                panelLength = part.Length;
                panelWidth = part.Width;
                panelCutLength = part.CutLength;
                panelCutWidth = part.CutWidth;
            }
            #endregion

            #region 判断是否有加工
            if (part.Routings.Count == 0
                && part.VDrillings.Count == 0
                && part.HDrillings.Count == 0
                && part.Sawings.Count == 0)
            {
                this.hasMachining = false;
            }
            else { this.hasMachining = true; }
            #endregion

            this.HDrillings = part.HDrillings;

            if (part.VDrillings.Count(it => it.FaceNumber == 5) > 0
                || part.Routings.Count(it => it.OnFace5) > 0
                || part.Sawings.Count(it => it.OnFace5) > 0)//面5有加工模式
            {
                this.VDrillings = part.VDrillings.Where(it => it.FaceNumber == 5).ToList();
                this.VDrillingsFace6 = part.VDrillings.Where(it => it.FaceNumber == 6).ToList();
                this.Routings = part.Routings.Where(it => it.OnFace5).ToList();
                this.RoutingsFace6 = part.Routings.Where(it => !it.OnFace5).ToList();
                this.Sawings = part.Sawings.Where(it => it.OnFace5).ToList();
                this.SawingsFace6 = part.Sawings.Where(it => !it.OnFace5).ToList();

                if (this.VDrillingsFace6.Count > 0 || this.RoutingsFace6.Count > 0 || this.SawingsFace6.Count > 0)
                {
                    this.MachiningType = PartMachiningType.DoubleFace;
                }
                else
                {
                    this.MachiningType = PartMachiningType.SingleFace5;
                }
            }
            else//只有面6的情况，需要翻转
            {
                this.VDrillings = part.VDrillings;
                this.Routings = part.Routings;
                this.Sawings = part.Sawings;

                if (part.VDrillings.Count == 0 && part.Routings.Count == 0 && part.Sawings.Count == 0)
                {
                    this.MachiningType = PartMachiningType.SingleFace5;
                }
                else
                {
                    this.MachiningType = PartMachiningType.SingleFace6;
                }
            }
        }

        public void Export()
        {
            if (!this.hasMachining)
            {
                return;
            }

            StringBuilder sb = new StringBuilder();
            getBorderString(sb, true);
            getHdrillString(sb);
            getVdrillString(sb, this.VDrillings);
            getRoutingString(sb, this.Routings);

            using (FileStream fs = new FileStream(Path.Combine(this.path, part.FileName + ".csv"), FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
            {
                sw.Write(sb.ToString());
            }

            if (MachiningType == PartMachiningType.DoubleFace)
            {
                StringBuilder sb2 = new StringBuilder();
                getBorderString(sb2, false);
                getVdrillString(sb2, this.VDrillingsFace6);
                getRoutingString(sb, this.RoutingsFace6);

                using (FileStream fs = new FileStream(Path.Combine(this.path, part.Face6FileName + ".csv"), FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
                {
                    sw.Write(sb2.ToString());
                }
            }
        }

        private void getRoutingString(StringBuilder sb, List<Routing> routings)
        {
            foreach (var r in routings)
            {
                Tool tool = Context.GetContext().MVDataContext
                    .GetLatestRelease().CurrentToolFile
                    .GetRouteToolByName(r.ToolName);

                if (tool == null)
                {
                    throw new Exception("未找到合适的刀具:" + r.ToolName);
                }

                double offsetStartX = 0, offsetStartY = 0;
                getOffsetStartXY(out offsetStartX, out offsetStartY, r, tool.Diameter);

                sb.Append("RouteSetMillSequence,");
                sb.Append(r.Points[0].X);
                sb.Append(",");
                sb.Append(r.Points[0].Y);
                sb.Append(",");
                sb.Append(r.Points[0].Z);
                sb.Append(",");
                sb.Append(offsetStartX);
                sb.Append(",");//start offsetx
                sb.Append(offsetStartY);
                sb.Append(",");//startoofsety
                sb.Append(tool.Diameter);
                sb.Append(",");
                sb.Append(r.ToolName);
                sb.Append(",");
                sb.Append(",");//previous toolname
                sb.Append(",");//next toolname
                sb.Append(",");//feed speed
                sb.Append(",");//entry spped
                sb.Append(",");//bit type
                sb.Append(",");//rotation

                if (r.ToolComp == ToolComp.None)
                {
                    sb.Append("0,");
                }
                else if (r.ToolComp == ToolComp.Left)
                {
                    sb.Append("1,");
                }
                else
                {
                    sb.Append("2,");
                }

                sb.Append(r.Points[0].X);
                sb.Append(",");
                sb.Append(r.Points[0].Y);
                sb.Append(",");
                sb.Append(r.Points[0].Z);
                sb.Append(",");
                sb.Append(",");//end offset x
                sb.Append(",");//end offset y
                sb.Append("0,");//bulge
                sb.Append(",");//radius
                sb.Append(",");//route centerx
                sb.Append(",");//route centery
                sb.Append(",");//route nextx
                sb.Append(",");//route nexty
                sb.Append(",");//route previous x 
                sb.Append(",");//route previous y
                sb.Append(",");//route previous z
                sb.Append(",");//route bulge next
                sb.Append(",");//setmill counter
                sb.Append(",");//vector counter
                sb.Append(",");//vector count
                sb.Append(",");//route angle
                sb.Append(",");//previous feedspped
                sb.Append(",");//previous entryspeed
                sb.Append(",");//foundtab
                sb.Append(Environment.NewLine);

                for (int i = 1; i < r.Points.Count; i++)
                {
                    sb.Append("RouteSequence,");
                    sb.Append(r.Points[0].X);
                    sb.Append(",");
                    sb.Append(r.Points[0].Y);
                    sb.Append(",");
                    sb.Append(r.Points[0].Z);
                    sb.Append(",");
                    sb.Append(",");//start offsetx
                    sb.Append(",");//startoofsety
                    sb.Append(tool.Diameter);
                    sb.Append(",");
                    sb.Append(r.ToolName);
                    sb.Append(",");
                    sb.Append(",");//previous toolname
                    sb.Append(",");//next toolname
                    sb.Append(",");//feed speed
                    sb.Append(",");//entry spped
                    sb.Append(",");//bit type
                    sb.Append(",");//rotation

                    if (r.ToolComp == ToolComp.None)
                    {
                        sb.Append("0,");
                    }
                    else if (r.ToolComp == ToolComp.Left)
                    {
                        sb.Append("1,");
                    }
                    else
                    {
                        sb.Append("2,");
                    }

                    sb.Append(r.Points[i].X);
                    sb.Append(",");
                    sb.Append(r.Points[i].Y);
                    sb.Append(",");
                    sb.Append(r.Points[i].Z);
                    sb.Append(",");
                    sb.Append(",");//end offset x
                    sb.Append(",");//end offset y
                    sb.Append(r.Bulges[i - 1]);//bulge
                    sb.Append(",");//radius
                    sb.Append(",");//route centerx
                    sb.Append(",");//route centery
                    sb.Append(",");//route nextx
                    sb.Append(",");//route nexty
                    sb.Append(",");//route previous x 
                    sb.Append(",");//route previous y
                    sb.Append(",");//route previous z
                    sb.Append(",");//route bulge next
                    sb.Append(",");//setmill counter
                    sb.Append(",");//vector counter
                    sb.Append(",");//vector count
                    sb.Append(",");//route angle
                    sb.Append(",");//previous feedspped
                    sb.Append(",");//previous entryspeed
                    sb.Append(",");//foundtab
                    sb.Append(Environment.NewLine);
                }
            }
        }

        private void getOffsetStartXY(out double offsetStartX, out double offsetStartY, Routing r, double dia)
        {
            if (r.ToolComp == ToolComp.None)//无偏移
            {
                offsetStartX = r.Points[0].X;
                offsetStartY = r.Points[0].Y;
                return;
            }


            using (Polyline pline = new Polyline())
            {
                pline.AddVertexAt(0, new Point2d(r.Points[0].X, r.Points[0].Y), r.Bulges[0], 0, 0);
                pline.AddVertexAt(1, new Point2d(r.Points[1].X, r.Points[1].Y), r.Bulges[1], 0, 0);

                double offset = dia / 2;
                if (r.ToolComp == ToolComp.Right)
                {
                    offset = -offset;
                }

                DBObjectCollection dbos = pline.GetOffsetCurves(offset);
                Curve curve = dbos[0] as Curve;
                offsetStartX = curve.StartPoint.X;
                offsetStartY = curve.StartPoint.Y;
                dbos.Dispose();
                curve.Dispose();
            }

        }

        private void getHdrillString(StringBuilder sb)
        {
            foreach (var hd in HDrillings)
            {
                sb.Append("HDrillSequence,");
                sb.Append(hd.FaceNumber);
                sb.Append(",");
                sb.Append(",");//previous face

                if (hd.FaceNumber == 1 || hd.FaceNumber == 2)
                {
                    sb.Append(hd.Position);
                    sb.Append(",");
                    sb.Append(hd.Depth);
                    sb.Append(",");
                }
                else
                {
                    sb.Append(hd.Depth);
                    sb.Append(",");
                    sb.Append(hd.Position);
                    sb.Append(",");
                }
                sb.Append(hd.ZValue);
                sb.Append(",");
                sb.Append(hd.Diameter);
                sb.Append(",");
                sb.Append(",");//hdrilltoolname
                sb.Append(",");//feedspeed
                sb.Append(",");//entryspeed
                sb.Append(",");//bittype
                sb.Append(",");//firstDrillDone
                sb.Append(",");//previousToolname
                sb.Append(",");//nexttoolname
                sb.Append(Environment.NewLine);
            }
        }

        private void getVdrillString(StringBuilder sb, IEnumerable<VDrilling> drillings)
        {
            foreach (var vd in drillings)
            {
                sb.Append("VdrillSequence,");
                sb.Append(vd.DimX);
                sb.Append(",");
                sb.Append(vd.DimY);
                sb.Append(",");
                sb.Append(vd.Depth);
                sb.Append(",");
                sb.Append("0,");//vdrillx offset
                sb.Append("0,");//vdrilly offset
                sb.Append(vd.Diameter);
                sb.Append(",");
                sb.Append(",");//vdrill toolname
                sb.Append(",");
                sb.Append(",");
                sb.Append(",");
                sb.Append(",");
                sb.Append(",");
                sb.Append(",");
                sb.Append(",");
                sb.Append(",");
                sb.Append(",");
                sb.Append(",");
                sb.Append(",");
                sb.Append(",");
                sb.Append(",");
                sb.Append(",");
                sb.Append(",");
                sb.Append(",");
                sb.Append(Environment.NewLine);
            }
        }

        private void getBorderString(StringBuilder sb, bool onFace5)
        {
            sb.Append("BorderSequence,");
            sb.Append(panelWidth);
            sb.Append(",");
            sb.Append(panelLength);
            sb.Append(",");
            sb.Append(part.Thickness);
            sb.Append(",");
            sb.Append("3");//runfield
            sb.Append(",");
            sb.Append(",");//currentface
            sb.Append(",");//previousface

            if (onFace5)
            {
                if (!part.MachinePoint.MP.EndsWith("M"))//普通
                {
                    if (this.MachiningType == PartMachiningType.SingleFace6)
                    {
                        sb.Append("DA");
                    }
                    else
                    {
                        sb.Append("AD");
                    }
                }
                else
                {
                    if (this.MachiningType == PartMachiningType.SingleFace6)
                    {
                        sb.Append("AD");
                    }
                    else
                    {
                        sb.Append("DA");
                    }
                }
            }
            else
            {
                if (!part.MachinePoint.MP.EndsWith("M"))//普通
                {
                    sb.Append("DA");
                }
                else
                {
                    sb.Append("AD");
                }
            }
            sb.Append(",");

            sb.Append(",");//FieldOffsetX
            sb.Append(",");//FieldOffsetY
            sb.Append(",");//FieldOffsetZ
            sb.Append(",");//JobName
            sb.Append(",");//ItemNumber
            sb.Append(part.FileName);//Filename
            sb.Append(",");
            sb.Append(part.Face6FileName);//Face6FileName
            sb.Append(",");
            sb.Append(part.PartName.Replace(",", ";"));
            sb.Append(",");
            sb.Append("1,");//partqty
            sb.Append(panelCutWidth);
            sb.Append(",");
            sb.Append(part.CutLength);
            sb.Append(",");
            sb.Append(part.Material.Name.Replace(",", ";"));
            sb.Append(",");
            sb.Append(part.Material.Code.Replace(",", ";"));
            sb.Append(",");

            if (!string.IsNullOrEmpty(part.EBW1.Name))
            {
                sb.Append(part.EBW1.Name.Replace(",", ";"));
            }
            sb.Append(",");

            if (!string.IsNullOrEmpty(part.EBW2.Name))
            {
                sb.Append(part.EBW2.Name.Replace(",", ";"));
            }
            sb.Append(",");

            if (!string.IsNullOrEmpty(part.EBL1.Name))
            {
                sb.Append(part.EBL1.Name.Replace(",", ";"));
            }
            sb.Append(",");

            if (!string.IsNullOrEmpty(part.EBL2.Name))
            {
                sb.Append(part.EBL2.Name.Replace(",", ";"));
            }
            sb.Append(",");

            sb.Append(part.Comment.Replace(",", ";") + "|" + part.Comment2.Replace(",", ";") + "|" + part.Comment3.Replace(",", ";"));
            sb.Append(",");
            sb.Append(part.Product.Description.Replace(",", ";"));//product description
            sb.Append(",");
            sb.Append("1,");//product qty
            sb.Append(part.Product.Width);
            sb.Append(",");
            sb.Append(part.Product.Height);
            sb.Append(",");
            sb.Append(part.Product.Depth);
            sb.Append(",");
            sb.Append(part.Product.Comments.Replace(",", ";"));
            sb.Append(",");
            sb.Append(",");//perfect grain
            sb.Append(",");//grainflag
            sb.Append(",");//part counter

            if (this.HDrillings.Count > 0)
            {
                sb.Append("TRUE");
            }
            else
            {
                sb.Append("FALSE");
            }
            sb.Append(",");

            if (this.VDrillings.Count > 0)
            {
                sb.Append("TRUE");
            }
            else
            {
                sb.Append("FALSE");
            }
            sb.Append(",");

            if (this.VDrillingsFace6.Count > 0)
            {
                sb.Append("TRUE");
            }
            else
            {
                sb.Append("FALSE");
            }
            sb.Append(",");

            if (this.Routings.Count > 0)
            {
                sb.Append("TRUE");
            }
            else
            {
                sb.Append("FALSE");
            }
            sb.Append(",");

            if (this.RoutingsFace6.Count > 0)
            {
                sb.Append("TRUE");
            }
            else
            {
                sb.Append("FALSE");
            }
            sb.Append(",");

            if (this.Sawings.Count > 0)
            {
                sb.Append("TRUE");
            }
            else
            {
                sb.Append("FALSE");
            }
            sb.Append(",");

            if (this.SawingsFace6.Count > 0)
            {
                sb.Append("TRUE");
            }
            else
            {
                sb.Append("FALSE");
            }
            sb.Append(",");

            if (onFace5)
            {
                sb.Append("FALSE");//found face6 program}
            }
            else
            {
                sb.Append("TRUE");
            }
            sb.Append(",");
            sb.Append("FALSE");//found nesting
            sb.Append(",");
            sb.Append(",");//firstPassDepth
            sb.Append(",");//SpoilBoardPenetration
            sb.Append(",");//unknown
            sb.Append(this.part.MachinePoint.MP);//machine point
            sb.Append(",");
            sb.Append(",");//release path
            sb.Append(",");//unknown
            sb.Append(",");//unknown
            sb.Append(",");//unknown
            sb.Append(",");//unknown
            sb.Append(",");//unknown
            sb.Append(",");//unknown
            sb.Append(",");//unknown
            sb.Append(",");//unknown
            sb.Append(",");//unknown
            sb.Append(",");//unknown
            sb.Append(",");//unknown
            sb.Append(",");//unknown
            sb.Append(",");//unknown
            sb.Append(",");//unknown
            sb.Append(Environment.NewLine);
        }
    }
}
