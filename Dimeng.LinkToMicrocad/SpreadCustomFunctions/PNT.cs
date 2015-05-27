using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.SpreadCustomFunctions
{
    //命令PNT
    public class PNT : SpreadsheetGear.CustomFunctions.Function
    {
        public static readonly PNT pnt = new PNT();
        public override void Evaluate(SpreadsheetGear.CustomFunctions.IArguments arguments, SpreadsheetGear.CustomFunctions.IValue result)
        {
            if (arguments.Count == 3)
            {
                string x = arguments.GetNumber(0).ToString();
                string y = arguments.GetNumber(1).ToString();
                string z = arguments.GetNumber(2).ToString();

                result.Text = x + ";" + y + ";" + z;
            }
            else
            {
                result.Error = SpreadsheetGear.ValueError.Value;
            }
        }
        private PNT()
            : base("PNT", SpreadsheetGear.CustomFunctions.Volatility.Invariant, SpreadsheetGear.CustomFunctions.ValueType.Text)
        { }
    }

}
