using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities
{
    public struct HardwareType
    {
        public static HardwareType Default()
        {
            return new HardwareType();
        }

        public HardwareType(string name, string code, List<HardwareToken> tokens)
        {
            this.HardwareName = name;
            this.HardwareCode = code;
            this.Tokens = tokens;
        }

        public string HardwareName;
        public string HardwareCode;
        public List<HardwareToken> Tokens;
    }

    public struct HardwareToken
    {
        public string TokenName;
        public string Par1;
        public string Par2;
        public string Par3;
        public string Par4;
        public string Par5;
        public string Par6;
        public string Par7;
        public string Par8;
        public string Par9;

        public HardwareToken(string token,string p1,string p2,string p3,string p4,string p5,string p6,string p7,string p8,string p9)
        {
            TokenName = token;
            Par1 = p1;
            Par2 = p2;
            Par3 = p3;
            Par4 = p4;
            Par5 = p5;
            Par6 = p6;
            Par7 = p7;
            Par8 = p8;
            Par9 = p9;
        }
    }
}
