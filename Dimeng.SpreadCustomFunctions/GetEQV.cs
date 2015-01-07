using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.SpreadCustomFunctions
{
    //命令GetEqv
    public class GetEQV : SpreadsheetGear.CustomFunctions.Function
    {
        public static readonly GetEQV geteqv = new GetEQV();

        public override void Evaluate(SpreadsheetGear.CustomFunctions.IArguments arguments, SpreadsheetGear.CustomFunctions.IValue result)
        {
            if (arguments.Count == 6)
            {
                double number1 = arguments.GetNumber(0);
                double number2 = arguments.GetNumber(1);
                double number3 = arguments.GetNumber(2);
                double number4 = arguments.GetNumber(3);
                double number5 = arguments.GetNumber(4);
                double number6 = arguments.GetNumber(5);
                result.Text = "EQV " + number1.ToString() + "," + number2.ToString() + "," + number3.ToString() + "," + number4.ToString() + "," + number5.ToString() + "," + number6.ToString();
            }
            else if (arguments.Count == 4)
            {
                double number1 = arguments.GetNumber(0);
                double number2 = arguments.GetNumber(1);
                double number3 = arguments.GetNumber(2);
                double number4 = arguments.GetNumber(3);
                result.Text = string.Format("EQV {0},{1},{2},{3}", number1, number2, number3, number4);
            }
            else
            {
                result.Error = SpreadsheetGear.ValueError.Value;
            }
        }

        private GetEQV()
            : base("GetEqv", SpreadsheetGear.CustomFunctions.Volatility.Invariant, SpreadsheetGear.CustomFunctions.ValueType.Text)
        { }
    }
}
