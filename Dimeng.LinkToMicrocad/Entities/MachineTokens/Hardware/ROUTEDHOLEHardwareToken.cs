using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class ROUTEDHOLEHardwareToken : HardwareToken
    {
        public ROUTEDHOLEHardwareToken(string token, string p1, string p2, string p3, string p4, string p5, string p6, string p7, string p8, string p9)
            : base(token, p1, p2, p3, p4, p5, p6, p7, p8, p9)
        {
        }
        public double StartX { get; private set; }
        public double StartY { get; private set; }
        public double StartZ { get; private set; }
        public double Raidus { get; private set; }
        public double RouteDepth { get; private set; }
        public bool IsPocket { get; private set; }
        public string ToolName { get; private set; }
        public int SequenceNumber { get; private set; }
    }
}
