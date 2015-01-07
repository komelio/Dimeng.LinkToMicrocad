using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.SpreadCustomFunctions
{
    //命令GETBULGE
    public class GETBULGE : SpreadsheetGear.CustomFunctions.Function
    {
        public static readonly GETBULGE getbulge = new GETBULGE();
        public override void Evaluate(SpreadsheetGear.CustomFunctions.IArguments arguments, SpreadsheetGear.CustomFunctions.IValue result)
        {
            if (arguments.Count == 2)
            {
                double degree = 2 * arguments.GetNumber(0) / arguments.GetNumber(1);

                result.Text = degree.ToString();
            }
            else
            {
                result.Error = SpreadsheetGear.ValueError.Value;
            }
        }
        private GETBULGE()
            : base("GetBulge", SpreadsheetGear.CustomFunctions.Volatility.Invariant, SpreadsheetGear.CustomFunctions.ValueType.Text)
        { }
    }
}
