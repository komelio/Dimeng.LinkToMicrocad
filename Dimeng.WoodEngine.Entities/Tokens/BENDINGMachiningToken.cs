using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class BENDINGMachiningToken:BaseToken
    {
        public BENDINGMachiningToken(string token, string par1, string par2, string par3, string par4, string par5, string par6, string par7, string par8, string par9, int row, int column, Part p)
            : base(token, par1, par2, par3, par4, par5, par6, par7, par8, par9, row, column, p)
        {
            Part.IsBend = true;

            Part.BendInfo = new BendingInfo();
            Part.BendInfo.Radius = Convert.ToDouble(par1);

            double isLongSide;
            if (double.TryParse(par2, out isLongSide))
            {
                if (isLongSide == 1)
                    Part.BendInfo.IsLongSide = true;
                else Part.BendInfo.IsLongSide = false;
            }
            else Part.BendInfo.IsLongSide = false;

            double angle;
            if(double.TryParse(par3,out angle))
            {
                Part.BendInfo.Angle = angle;
            }
            else Part.BendInfo.Angle = 0;
        }
    }
}
