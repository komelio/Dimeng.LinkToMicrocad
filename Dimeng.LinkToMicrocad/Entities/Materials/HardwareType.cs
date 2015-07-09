using Dimeng.WoodEngine.Entities.MachineTokens;
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
}
