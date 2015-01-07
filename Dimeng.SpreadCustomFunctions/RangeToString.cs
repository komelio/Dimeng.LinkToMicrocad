using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.SpreadCustomFunctions
{
    //命令RANGETOSTRING
    public class RANGETOSTRING : SpreadsheetGear.CustomFunctions.Function
    {
        public static readonly RANGETOSTRING rangetostring = new RANGETOSTRING();

        public override void Evaluate(SpreadsheetGear.CustomFunctions.IArguments arguments, SpreadsheetGear.CustomFunctions.IValue result)
        {
            if (arguments.Count == 2 || arguments.Count == 3)
            {
                string pipeline = arguments.GetText(1);
                string last = "";
                int rowCount, columnCount;
                arguments.GetArrayDimensions(0, out rowCount, out columnCount);
                last = arguments.GetArrayValue(0, 0, 0).Text;
                for (int i = 1; i < rowCount; i++)
                {
                    SpreadsheetGear.CustomFunctions.IValue value = arguments.GetArrayValue(0, i, 0);
                    last = last + pipeline + value.Text;
                }
                result.Text = last;
            }
            else
            {
                result.Error = SpreadsheetGear.ValueError.Value;
            }
        }

        private RANGETOSTRING()
            : base("RANGETOSTRING", SpreadsheetGear.CustomFunctions.Volatility.Invariant, SpreadsheetGear.CustomFunctions.ValueType.Number)
        { }
    }
}
