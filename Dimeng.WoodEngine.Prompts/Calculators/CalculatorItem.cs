using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Prompts
{
    public class CalculatorItem
    {
        public string Name { get; set; }
        public double UpperBound { get; set; }
        public double Gap { get; set; }

        public int Index { get; set; }
    }
}
