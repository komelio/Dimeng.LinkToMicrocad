using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.SpreadCustomFunctions
{
    public class Partlength : SpreadsheetGear.CustomFunctions.Function
    {
        public static readonly Partlength PL = new Partlength();

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
                        result.Number = arguments.GetArrayValue(0, i, 3).Number;
                        break;
                    }
                }

            }
            else
            {
                result.Error = SpreadsheetGear.ValueError.Value;
            }
        }

        private Partlength()
            : base("PartLength", SpreadsheetGear.CustomFunctions.Volatility.Invariant, SpreadsheetGear.CustomFunctions.ValueType.Number)
        { }
    }
}
