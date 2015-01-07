using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.SpreadCustomFunctions
{

    //命令GETARCCHORD
    public class GETARCCHORD : SpreadsheetGear.CustomFunctions.Function
    {
        public static readonly GETARCCHORD getarcchord = new GETARCCHORD();
        public override void Evaluate(SpreadsheetGear.CustomFunctions.IArguments arguments, SpreadsheetGear.CustomFunctions.IValue result)
        {
            if (arguments.Count == 2)
            {
                double degree = 2 * arguments.GetNumber(0) * Math.Sin(arguments.GetNumber(1) / 2 / 180 * Math.PI);

                result.Text = degree.ToString();
            }
            else
            {
                result.Error = SpreadsheetGear.ValueError.Value;
            }
        }
        private GETARCCHORD()
            : base("GETARCCHORD", SpreadsheetGear.CustomFunctions.Volatility.Invariant, SpreadsheetGear.CustomFunctions.ValueType.Text)
        { }
    }
}
