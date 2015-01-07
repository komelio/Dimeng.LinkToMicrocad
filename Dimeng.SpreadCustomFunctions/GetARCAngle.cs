using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.SpreadCustomFunctions
{
    //命令GETARCANGLE
    public class GETARCANGLE : SpreadsheetGear.CustomFunctions.Function
    {
        public static readonly GETARCANGLE getarcangle = new GETARCANGLE();
        public override void Evaluate(SpreadsheetGear.CustomFunctions.IArguments arguments, SpreadsheetGear.CustomFunctions.IValue result)
        {
            if (arguments.Count == 2)
            {
                double degree = 4 * Math.Atan(2 * arguments.GetNumber(0) / arguments.GetNumber(1)) / Math.PI * 180;

                result.Text = degree.ToString();
            }
            else
            {
                result.Error = SpreadsheetGear.ValueError.Value;
            }
        }
        private GETARCANGLE()
            : base("GETARCANGLE", SpreadsheetGear.CustomFunctions.Volatility.Invariant, SpreadsheetGear.CustomFunctions.ValueType.Text)
        { }
    }
}
