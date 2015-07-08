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
        public MITERMachiningToken(string token, string par1, string par2, string par3, string par4, string par5, string par6, string par7, string par8, string par9)
            : base(token, par1, par2, par3, par4, par5, par6, par7, par8, par9)
        {
            IsDrawOnly = true;
        }

        public override bool Valid(Checks.MachineTokenChecker check)
        {
            this.FaceNumber = check.FaceNumber(this.Token, 6, new int[] { 1, 2, 3, 4, 5, 6, 7, 8 });
            this.EdgeNumber = check.EdgeNumber(this.Token, 8, new int[] { 1, 2, 3, 4, 5, 6, 7, 8 });

            if (FaceNumber == EdgeNumber)
            {
                this.Errors.Add(new ModelError("MITER", "面与边不能相等", ErrorLevel.Error));
            }

            Angle = check.GetDoubleValue(Par1, "MITER/角度", true, this.Errors);
            if (Angle >= 90 || Angle < 0)
            {
                this.Errors.Add(new ModelError("MITER", "角度范围在0到90度之间", ErrorLevel.Error));
            }
            Angle = Angle / 180 * System.Math.PI;

            if (this.Errors.Count == 0)
            {
                return true;
            }
            else return false;
        }

        public double Angle { get; set; }

        public override void ToMachining(double AssociatedDist, ToolFile toolFile)
        {
            Profile profile = new Profile();
            profile.IsSharpFromFile = false;

            profile.StartPointNumber = this.FaceNumber;
            profile.EndPointNumber = this.EdgeNumber;

            profile.Angle = this.Angle;

            this.Part.Profiles.Add(profile);
        }
    }
}
