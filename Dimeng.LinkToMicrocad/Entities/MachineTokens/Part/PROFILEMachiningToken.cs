using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Dimeng.WoodEngine.Entities.Machinings;
using Dimeng.WoodEngine.Entities.Checks;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class PROFILEMachiningToken : UnAssociativeToken
    {
        public PROFILEMachiningToken(string token, string par1, string par2, string par3, string par4, string par5, string par6, string par7, string par8, string par9)
            : base(token, par1, par2, par3, par4, par5, par6, par7, par8, par9)
        {
            this.IsDrawOnly = true;
        }

        public override bool Valid(MachineTokenChecker check)
        {
            this.FaceNumber = check.FaceNumber(this.Token, 8, new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }, this.Errors);
            this.EdgeNumber = check.EdgeNumber(this.Token, 10, new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }, this.Errors);

            if (FaceNumber == EdgeNumber)
            {
                this.Errors.Add(new ModelError("PROFILE指令的两个点不能相同:" + this.Token));
            }

            ProfileFile = check.TokenFileName(this.Par1, "PROFILE/文件名", this.Errors);

            if (this.Errors.Count > 0)
            {
                return false;
            }
            return true;
        }

        public string ProfileFile { get; private set; }

        public override void ToMachining(double AssociatedDist, ToolFile toolFile)
        {
            Profile profile = new Profile();
            profile.IsSharpFromFile = true;
            profile.SharpFile = this.ProfileFile;

            profile.StartPointNumber = this.FaceNumber;
            profile.EndPointNumber = this.EdgeNumber;

            this.Part.Profiles.Add(profile);
        }
    }
}
