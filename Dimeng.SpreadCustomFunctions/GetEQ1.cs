using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.SpreadCustomFunctions
{

    //命令GetEQ1
    public class GetEQ1 : SpreadsheetGear.CustomFunctions.Function
    {
        public static readonly GetEQ1 geteq1 = new GetEQ1();

        public override void Evaluate(SpreadsheetGear.CustomFunctions.IArguments arguments, SpreadsheetGear.CustomFunctions.IValue result)
        {
            if (arguments.Count == 2)
            {
                double number1 = arguments.GetNumber(0);
                double number2 = arguments.GetNumber(1);
                result.Text = "EQ1 " + number1.ToString() + "," + number2.ToString();
            }
            else
            {
                result.Error = SpreadsheetGear.ValueError.Value;
            }
        }

        private GetEQ1()
            : base("GETEQ1", SpreadsheetGear.CustomFunctions.Volatility.Invariant, SpreadsheetGear.CustomFunctions.ValueType.Text)
        { }
    }
}
