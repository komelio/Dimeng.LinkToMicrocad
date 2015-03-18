using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities
{
    public class BendingInfo
    {
        public double Radius { get; set; }
        public bool IsLongSide { get; set; }
        public double Angle { get; set; }
        public string ToolName { get; set; }
    }
}
