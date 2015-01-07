﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.SpreadCustomFunctions
{
    //命令PartWidth
    public class Partwidth : SpreadsheetGear.CustomFunctions.Function
    {
        public static readonly Partwidth pw = new Partwidth();

        public override void Evaluate(SpreadsheetGear.CustomFunctions.IArguments arguments, SpreadsheetGear.CustomFunctions.IValue result)
        {
            if (arguments.Count == 2)
            {
                string partname = arguments.GetText(1);
                int rowCount, columnCount;
                arguments.GetArrayDimensions(0, out rowCount, out columnCount);
                for (int i = 0; i < rowCount; i++)
                {
                    SpreadsheetGear.CustomFunctions.IValue value = arguments.GetArrayValue(0, i, 0);
                    if (value.Text == partname)
                    {
                        result.Number = arguments.GetArrayValue(0, i, 2).Number;
                        break;
                    }
                }

            }
            else
            {
                result.Error = SpreadsheetGear.ValueError.Value;
            }
        }

        private Partwidth()
            : base("PartWidth", SpreadsheetGear.CustomFunctions.Volatility.Invariant, SpreadsheetGear.CustomFunctions.ValueType.Number)
        { }
    }
}
