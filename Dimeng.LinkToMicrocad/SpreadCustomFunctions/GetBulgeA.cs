using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.SpreadCustomFunctions
{
    //命令GETBULGEA
    public class GETBULGEA : SpreadsheetGear.CustomFunctions.Function
    {
        public static readonly GETBULGEA getbulgea = new GETBULGEA();
        public override void Evaluate(SpreadsheetGear.CustomFunctions.IArguments arguments, SpreadsheetGear.CustomFunctions.IValue result)
        {
            if (arguments.Count == 1)
            {
                double degree = System.Math.Tan(arguments.GetNumber(0) / 4 / 180 * System.Math.PI);

                result.Text = degree.ToString();
            }
            else
            {
                result.Error = SpreadsheetGear.ValueError.Value;
            }
        }
        private GETBULGEA()
            : base("GETBULGEA", SpreadsheetGear.CustomFunctions.Volatility.Invariant, SpreadsheetGear.CustomFunctions.ValueType.Text)
        { }
    }
}
