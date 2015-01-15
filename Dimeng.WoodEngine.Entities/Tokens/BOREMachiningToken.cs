using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class BOREMachiningToken : BaseToken
    {
        public BOREMachiningToken(string token, string par1, string par2, string par3, string par4, string par5, string par6, string par7, string par8, string par9, int row, int column, Part p)
            : base(token, par1, par2, par3, par4, par5, par6, par7, par8, par9, row, column, p)
        {
            IsDrawOnly = false;
        }

        public override bool Valid(Logger logger)
        {
            this.logger = logger;

            base.faceNumberChecker(this.Token, 4, new int[] { 1, 2, 3, 4, 5, 6 });
            PosStartX = base.DoubleChecker(this.Par1, @"BORE/X起始坐标", false);
            PosStartY = base.DoubleChecker(this.Par2, @"BORE/Y起始坐标", false);
            PosStartZ = base.DoubleChecker(this.Par3, @"BORE/Z起始坐标", true);
            Diameter = base.DoubleChecker(this.Par4, @"BORE/孔直径", false);
            PosEndX = (string.IsNullOrEmpty(Par5)) ? PosStartX : base.DoubleChecker(this.Par5, @"BORE/X结束坐标", false);
            PosEndY = (string.IsNullOrEmpty(Par6)) ? PosStartY : base.DoubleChecker(this.Par6, @"BORE/Y结束坐标", false);
            HoleGap = (string.IsNullOrEmpty(Par7)) ? 0 : base.DoubleChecker(this.Par5, @"BORE/孔间距", true);

            if (PosEndX == PosStartX &&
                PosStartY == PosEndY)
            {
                IsLineHoles = true;
            }

            CheckFaceNumber();

            return this.IsValid;
        }

        private void CheckFaceNumber()
        {
            if (FaceNumber == 1 || FaceNumber == 2 ||
                FaceNumber == 3 || FaceNumber == 4)
            {
                IsHorizontalDrill = true;
            }
            else if (FaceNumber == 5 || FaceNumber == 6)
                IsHorizontalDrill = false;
        }

        public double PosStartX { get; private set; }
        public double PosStartY { get; private set; }
        public double PosStartZ { get; private set; }
        public double Diameter { get; private set; }
        public double PosEndX { get; private set; }
        public double PosEndY { get; private set; }
        public double HoleGap { get; private set; }

        public bool IsLineHoles { get; private set; }
        public bool IsHorizontalDrill { get; private set; }

        public override void ToMachining(double AssociatedDist, Entities.ToolFile toolFile)
        {
            int number = 1;
            double dist = System.Math.Sqrt(
                        System.Math.Abs(PosStartX - PosEndX) * System.Math.Abs(PosStartX - PosEndX) +
                        System.Math.Abs(PosStartY - PosEndY) * System.Math.Abs(PosStartY - PosEndY)
                        );
            if (HoleGap < 1)//避免太小的间距
                HoleGap = 1;

            number = (int)System.Math.Floor(dist / HoleGap) + 1;
            double xfactor = (PosStartX > PosEndX) ? -1 : 1;
            double yfactor = (PosStartY > PosEndY) ? -1 : 1;
            double cos = (dist != 0) ? System.Math.Abs(PosStartX - PosEndX) / dist : 0;
            double sin = (dist != 0) ? System.Math.Abs(PosStartY - PosEndY) / dist : 0;

            //如果是水平孔
            if (IsHorizontalDrill)
            {
                if (FaceNumber == 3)
                {
                    for (int i = 0; i < number; i++)
                    {
                        Machinings.HDrilling hdrill = new Machinings.HDrilling(
                            FaceNumber,
                            Diameter,
                            PosStartX,
                            PosStartY + dist / number * i * yfactor * sin,
                            PosStartZ,
                            Part);
                        Part.HDrillings.Add(hdrill);
                    }
                }
                else if (FaceNumber == 4)
                {
                    for (int i = 0; i < number; i++)
                    {
                        double depth = (Part.MachinePoint.IsRotated) ? Part.Width - PosStartX : Part.Length - PosStartX;
                        if (depth < 0)
                            throw new Exception("指令的深度不能为小于0的数值");

                        Machinings.HDrilling hdrill = new Machinings.HDrilling(
                            FaceNumber,
                            Diameter,
                            depth,
                            PosStartY + dist / number * i * yfactor * sin,
                            PosStartZ,
                            Part);
                        Part.HDrillings.Add(hdrill);
                    }
                }
                else if (FaceNumber == 1)
                {
                    for (int i = 0; i < number; i++)
                    {
                        Machinings.HDrilling hdrill = new Machinings.HDrilling(
                            FaceNumber,
                            Diameter,
                            PosStartY,
                            PosStartX + dist / number * i * xfactor * cos,
                            PosStartZ,
                            Part);
                        Part.HDrillings.Add(hdrill);
                    }
                }
                else if (FaceNumber == 2)
                {
                    for (int i = 0; i < number; i++)
                    {
                        double depth = (Part.MachinePoint.IsRotated) ? Part.Length - PosStartY : Part.Width - PosStartY;
                        Machinings.HDrilling hdrill = new Machinings.HDrilling(
                            FaceNumber,
                            Diameter,
                            depth,
                            PosStartX + dist / number * i * xfactor * cos,
                            PosStartZ,
                            Part);
                        Part.HDrillings.Add(hdrill);
                    }
                }
                else
                {
                    throw new Exception("未知的Facenumber:" + FaceNumber);
                }
            }
            else//如果是面孔
            {
                if (IsLineHoles)
                {
                    for (int i = 0; i < number; i++)
                    {
                        Machinings.VDrilling vdrill = new Machinings.VDrilling(
                            FaceNumber,
                            PosStartX + dist / number * i * xfactor * cos,
                            PosStartY + dist / number * i * yfactor * sin,
                            Diameter,
                            PosStartZ,
                            Part);

                        Part.VDrillings.Add(vdrill);
                    }
                }

                Part.VDrillings.Add(new Machinings.VDrilling(this.FaceNumber, PosStartX, PosStartY, Diameter, PosStartZ, Part));
            }
        }
    }
}
