using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities
{
    public class HardwareToken
    {
        public string TokenName { get; private set; }
        public string Par1 { get; private set; }
        public string Par2 { get; private set; }
        public string Par3 { get; private set; }
        public string Par4 { get; private set; }
        public string Par5 { get; private set; }
        public string Par6 { get; private set; }
        public string Par7 { get; private set; }
        public string Par8 { get; private set; }
        public string Par9 { get; private set; }

        public HardwareToken(string token, string p1, string p2, string p3, string p4, string p5, string p6, string p7, string p8, string p9)
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
