using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Geometry;
using Dimeng.WoodEngine.Entities.Checks;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class ROUTEMachiningToken : BaseToken
    {
        public ROUTEMachiningToken(string token, string par1, string par2, string par3, string par4, string par5, string par6, string par7, string par8, string par9)
            : base(token, par1, par2, par3, par4, par5, par6, par7, par8, par9)
        {
        }

        public override bool Valid(MachineTokenChecker check)
        {
            this.FaceNumber = check.FaceNumber(Token, 5, new int[] { 5, 6 });

            this.PosStartX = check.GetDoubleValue(Par1, "直线立铣/X起始位置", false, check.Errors);
            this.PosStartY = check.GetDoubleValue(Par2, "直线立铣/Y起始位置", false, check.Errors);
            this.StartDepth = check.GetDoubleValue(Par3, "直线立铣/起始深度", true, check.Errors);
            this.PosEndX = check.GetDoubleValue(Par4, "直线立铣/X终止位置", false, check.Errors);
            this.PosEndY = check.GetDoubleValue(Par5, "直线立铣/Y终止位置", false, check.Errors);
            this.EndDepth = check.GetDoubleValue(Par6, "直线立铣/终止深度", true, check.Errors);
            this.ToolName = check.ToolName(Par7, "直线立铣/刀具名称");

            if (Par8 == "R")
                ToolComp = ToolComp.Right;
            else if (Par8 == "L")
                ToolComp = ToolComp.Left;
            else ToolComp = ToolComp.None;

            IsDrawOnly = check.GetBoolValue(Par9, "直线立铣/仅用于绘图", false, false, check.Errors);

            if (check.Errors.Count == 0)
            {
                return true;
            }
            else return false;
        }

        public double PosStartX { get; private set; }
        public double PosStartY { get; private set; }
        public double StartDepth { get; private set; }
        public double PosEndX { get; private set; }
        public double PosEndY { get; private set; }
        public double EndDepth { get; private set; }
        public string ToolName { get; private set; }
        public ToolComp ToolComp { get; private set; }

        public override void ToMachining(double AssociatedDist, Entities.ToolFile toolFile)
        {
            Point3d pt1 = new Point3d(PosStartX, PosStartY, StartDepth);
            Point3d pt2 = new Point3d(PosEndX, PosEndY, EndDepth);
            List<Point3d> points = new List<Point3d>() { pt1, pt2 };
            List<double> bulges = new List<double>() { 0, 0 };
            Machinings.Routing route = new Machinings.Routing();
            route.Bulges = bulges;
            route.Points = points;
            route.ToolComp = this.ToolComp;
            route.Part = this.Part;
            route.OnFace5 = (this.FaceNumber == 5) ? true : false;
            route.ToolName = this.ToolName;

            Part.Routings.Add(route);
        }
    }
}
