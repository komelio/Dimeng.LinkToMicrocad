using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuoteExport.Dimensions
{
    public class DrillInfo
    {
        public DrillInfo(int id, float dia, float depth)
        {
            this.Index = id;
            this.Diameter = dia;
            this.Depth = depth;
        }
        public int Index { get; set; }
        public float Diameter { get; set; }
        public float Depth { get; set; }
    }
}
