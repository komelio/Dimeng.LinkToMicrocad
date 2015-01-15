using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dimeng.WoodEngine.Entities.Machinings;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class MITERMachiningToken :
        BaseToken
    {
        public MITERMachiningToken(string token, string par1, string par2, string par3, string par4, string par5, string par6, string par7, string par8, string par9, int row, int column, Part p)
            : base(token, par1, par2, par3, par4, par5, par6, par7, par8, par9, row, column, p)
        {
            IsDrawOnly = true;
        }

        public override bool Valid(Logger logger)
        {
            this.logger = logger;

            base.faceNumberChecker(this.Token, 6, new int[] { 1, 2, 3, 4, 5, 6, 7, 8 });
            base.edgeNumberChecker(this.Token, 8, new int[] { 1, 2, 3, 4, 5, 6, 7, 8 });

            if (FaceNumber == EdgeNumber)
            {
                this.IsValid = false;
                this.writeError("MITER指令的两个点不能相同:" + this.Token, false);
            }

            Angle = base.DoubleChecker(Par1, "MITER/角度", true);
            if (Angle >= 90 || Angle < 0)
                this.writeError("MITER的角度只能在0到90之间:" + this.Par1, false);

            Angle = Angle / 180 * System.Math.PI;

            return this.IsValid;
        }

        public double Angle { get; set; }

        public override void ToMachining(double AssociatedDist, ToolFile toolFile)
        {
            Profile profile = new Profile();
            profile.IsSharpFromFile = false;

            profile.StartPointNumber = this.FaceNumber.ToString();
            profile.EndPointNumber = this.EdgeNumber.ToString();

            profile.Angle = this.Angle;

            this.Part.Profiles.Add(profile);
        }
    }
}
