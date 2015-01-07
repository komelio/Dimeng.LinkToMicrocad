using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.SpreadCustomFunctions
{

    //命令GETARCALTITUDE
    public class GETARCALTITUDE : SpreadsheetGear.CustomFunctions.Function
    {
        public static readonly GETARCALTITUDE getarcaltitude = new GETARCALTITUDE();
        public override void Evaluate(SpreadsheetGear.CustomFunctions.IArguments arguments, SpreadsheetGear.CustomFunctions.IValue result)
        {
            if (arguments.Count == 2)
            {
                double degree = arguments.GetNumber(0) * (1 - Math.Cos(arguments.GetNumber(1) / 2 / 180 * Math.PI));

                result.Text = degree.ToString();
            }
            else
            {
                result.Error = SpreadsheetGear.ValueError.Value;
            }
        }
        private GETARCALTITUDE()
            : base("GETARCALTITUDE", SpreadsheetGear.CustomFunctions.Volatility.Invariant, SpreadsheetGear.CustomFunctions.ValueType.Text)
        { }
    }

}
