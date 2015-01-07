using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.SpreadCustomFunctions
{
    //命令GetEQH
    public class GetEQH : SpreadsheetGear.CustomFunctions.Function
    {
        public static readonly GetEQH geteqh = new GetEQH();

        public override void Evaluate(SpreadsheetGear.CustomFunctions.IArguments arguments, SpreadsheetGear.CustomFunctions.IValue result)
        {
            if (arguments.Count == 4)
            {
                double number1 = arguments.GetNumber(0);
                double number2 = arguments.GetNumber(1);
                double number3 = arguments.GetNumber(2);
                double number4 = arguments.GetNumber(3);
                result.Text = "EQH " + number1.ToString() + "," + number2.ToString() + "," + number3.ToString() + "," + number4.ToString();
            }
            else
            {
                result.Error = SpreadsheetGear.ValueError.Value;
            }
        }

        private GetEQH()
            : base("GETEQH", SpreadsheetGear.CustomFunctions.Volatility.Invariant, SpreadsheetGear.CustomFunctions.ValueType.Text)
        { }
    }
}
