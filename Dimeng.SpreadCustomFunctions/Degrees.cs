using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.SpreadCustomFunctions
{
    public class Degrees : SpreadsheetGear.CustomFunctions.Function
    {
        public static readonly Degrees degrees = new Degrees();
        public override void Evaluate(SpreadsheetGear.CustomFunctions.IArguments arguments, SpreadsheetGear.CustomFunctions.IValue result)
        {
            if (arguments.Count == 1)
            {
                double degree = arguments.GetNumber(0) / Math.PI * 180;

                result.Text = degree.ToString();
            }
            else
            {
                result.Error = SpreadsheetGear.ValueError.Value;
            }
        }
        private Degrees()
            : base("Degrees", SpreadsheetGear.CustomFunctions.Volatility.Invariant, SpreadsheetGear.CustomFunctions.ValueType.Text)
        { }
    }
}
