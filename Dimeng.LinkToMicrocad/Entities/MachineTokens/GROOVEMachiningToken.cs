using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class GROOVEMachiningToken : BaseToken
    {
        public GROOVEMachiningToken(string token, string par1, string par2, string par3, string par4, string par5, string par6, string par7, string par8, string par9, int row, int column, Part p)
            : base(token, par1, par2, par3, par4, par5, par6, par7, par8, par9, row, column, p)
        {
        }

        public override bool Valid(Logger logger)
        {
            this.logger = logger;

            base.faceNumberChecker(this.Token, 7, new int[] { 5, 6 });
            StartX = base.DoubleChecker(Par1, "锯槽/X起始坐标", false);
            StartY = base.DoubleChecker(Par2, "锯槽/Y起始坐标", false);
            StartDepth = base.DoubleChecker(Par3, "锯槽/起始深度", false);
            EndX = base.DoubleChecker(Par4, "锯槽/X结束始坐标", false);
            EndY = base.DoubleChecker(Par5, "锯槽/Y结束坐标", false);
            EndDepth = base.DoubleChecker(Par6, "锯槽/结束深度", false);
            ToolName = base.notEmptyStringChecker(Par7, "锯槽/刀具名称");

            if (Par8 == "R")
                this.ToolComp = ToolComp.Right;
            else if (Par8 == "L")
                this.ToolComp = ToolComp.Left;
            else this.ToolComp = ToolComp.None;

            IsDrawOnly = base.BoolChecker(Par9, "锯槽/仅用于绘图", false, false);

            return this.IsValid;
        }

        public double StartX { get; set; }
        public double StartY { get; set; }
        public double StartDepth { get; set; }
        public double EndX { get; set; }
        public double EndY { get; set; }
        public double EndDepth { get; set; }
        public string ToolName { get; set; }
        public ToolComp ToolComp { get; set; }

        public override void ToMachining(double AssociatedDist, ToolFile toolFile)
        {
            Machinings.Sawing sawing = new Machinings.Sawing();

            sawing.Part = this.Part;

            if (this.FaceNumber == 5)
                sawing.OnFace5 = true;
            else if (this.FaceNumber == 6)
                sawing.OnFace5 = false;

            sawing.StartX = this.StartX;
            sawing.StartY = this.StartY;
            sawing.StartDepth = this.StartDepth;
            sawing.EndX = this.EndX;
            sawing.EndY = this.EndY;
            sawing.EndDepth = this.EndDepth;
            sawing.ToolName = this.ToolName;
            sawing.IsDrawOnly = this.IsDrawOnly;
            sawing.ToolComp = this.ToolComp;

            Part.Sawings.Add(sawing);
        }
    }
}
