using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.SpreadCustomFunctions
{
    //命令GETARCRADIUS
    public class GETARCRADIUS : SpreadsheetGear.CustomFunctions.Function
    {
        public static readonly GETARCRADIUS getarcradius = new GETARCRADIUS();
        public override void Evaluate(SpreadsheetGear.CustomFunctions.IArguments arguments, SpreadsheetGear.CustomFunctions.IValue result)
        {
            if (arguments.Count == 2)
            {
                double degree = arguments.GetNumber(0) / (Math.PI / 180 * arguments.GetNumber(1));

                result.Text = degree.ToString();
            }
            else
            {
                result.Error = SpreadsheetGear.ValueError.Value;
            }
        }
        private GETARCRADIUS()
            : base("GETARCRADIUS", SpreadsheetGear.CustomFunctions.Volatility.Invariant, SpreadsheetGear.CustomFunctions.ValueType.Text)
        { }
    }
}
