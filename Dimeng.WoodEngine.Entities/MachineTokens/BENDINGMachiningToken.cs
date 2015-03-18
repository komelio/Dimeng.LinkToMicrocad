using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class BENDINGMachiningToken : BaseToken
    {
        public BENDINGMachiningToken(string token, string par1, string par2, string par3, string par4, string par5, string par6, string par7, string par8, string par9)
            : base(token, par1, par2, par3, par4, par5, par6, par7, par8, par9)
        {

        }

        public override bool Valid(Checks.MachineTokenChecker chekcer)
        {
            Part.IsBend = true;

            Part.BendingInfo = new BendingInfo();
            Part.BendingInfo.Radius = Convert.ToDouble(Par1);

            double isLongSide;
            if (double.TryParse(Par2, out isLongSide))
            {
                if (isLongSide == 1)
                    Part.BendingInfo.IsLongSide = true;
                else Part.BendingInfo.IsLongSide = false;
            }
            else Part.BendingInfo.IsLongSide = false;

            double angle;
            if (double.TryParse(Par3, out angle))
            {
                Part.BendingInfo.Angle = angle;
            }
            else Part.BendingInfo.Angle = 0;

            return true;
        }
    }
}
