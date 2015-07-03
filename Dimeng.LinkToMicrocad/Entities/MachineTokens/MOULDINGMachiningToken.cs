using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class MOLDINGMachiningToken : UnAssociativeToken
    {
        public MOLDINGMachiningToken(string token, string par1, string par2, string par3, string par4, string par5, string par6, string par7, string par8, string par9)
            : base(token, par1, par2, par3, par4, par5, par6, par7, par8, par9)
        {

        }

        public override bool Valid(Checks.MachineTokenChecker chekcer)
        {
            if (string.IsNullOrEmpty(Par1))
            {
                return false;
            }

            if (!Par1.ToUpper().EndsWith(".DWG"))
            {
                return false;
            }

            DWGFileName = this.Par1;
            this.Part.Molding = new Machinings.Molding(this.Par1);
            this.Part.IsMolding = true;

            return true;
        }

        public string DWGFileName { get; private set; }
    }
}
