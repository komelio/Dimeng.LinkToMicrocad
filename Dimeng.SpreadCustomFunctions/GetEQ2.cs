﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.SpreadCustomFunctions
{
    public class GetEQ2 : SpreadsheetGear.CustomFunctions.Function
    {
        public static readonly GetEQ2 geteq2 = new GetEQ2();

        public override void Evaluate(SpreadsheetGear.CustomFunctions.IArguments arguments, SpreadsheetGear.CustomFunctions.IValue result)
        {
            if (arguments.Count == 2)
            {
                double number1 = arguments.GetNumber(0);
                double number2 = arguments.GetNumber(1);
                result.Text = "EQ2 " + number1.ToString() + "," + number2.ToString();
            }
            else
            {
                result.Error = SpreadsheetGear.ValueError.Value;
            }
        }

        private GetEQ2()
            : base("GETEQ2", SpreadsheetGear.CustomFunctions.Volatility.Invariant, SpreadsheetGear.CustomFunctions.ValueType.Text)
        { }
    }
}
