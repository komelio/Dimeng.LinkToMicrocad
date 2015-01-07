using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.SpreadCustomFunctions
{
    //命令GETARCLENGTH
    public class GETARCLENGTH : SpreadsheetGear.CustomFunctions.Function
    {
        public static readonly GETARCLENGTH getarclength = new GETARCLENGTH();
        public override void Evaluate(SpreadsheetGear.CustomFunctions.IArguments arguments, SpreadsheetGear.CustomFunctions.IValue result)
        {
            if (arguments.Count == 2)
            {
                double degree = arguments.GetNumber(1) * Math.PI / 180 * arguments.GetNumber(0);

                result.Text = degree.ToString();
            }
            else
            {
                result.Error = SpreadsheetGear.ValueError.Value;
            }
        }
        private GETARCLENGTH()
            : base("GETARCLENGTH", SpreadsheetGear.CustomFunctions.Volatility.Invariant, SpreadsheetGear.CustomFunctions.ValueType.Text)
        { }
    }

}
