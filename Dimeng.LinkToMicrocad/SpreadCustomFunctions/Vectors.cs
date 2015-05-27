using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.SpreadCustomFunctions
{
    public class Vectors : SpreadsheetGear.CustomFunctions.Function
    {
        public static readonly Vectors vectors = new Vectors();
        public override void Evaluate(SpreadsheetGear.CustomFunctions.IArguments arguments, SpreadsheetGear.CustomFunctions.IValue result)
        {
            bool rs = true;
            string combinestring = "";
            for (int i = 0; i < arguments.Count; i++)
            {
                string argu = arguments.GetText(i);
                char[] chars = argu.ToCharArray();
                int number = 0;
                for (int p = 0; p < chars.Length; p++)
                {
                    if (chars[p] == ';') number++;
                }
                if (number == 2)
                {

                    if (i != arguments.Count - 1)
                    {
                        combinestring = combinestring + arguments.GetText(i) + "|";
                    }
                    else
                        combinestring = combinestring + arguments.GetText(i);
                }
                else
                {
                    rs = false;
                    break;
                }
            }
            if (rs)
                result.Text = combinestring;
            else
                result.Error = SpreadsheetGear.ValueError.Value;
        }
        private Vectors()
            : base("Vectors", SpreadsheetGear.CustomFunctions.Volatility.Invariant, SpreadsheetGear.CustomFunctions.ValueType.Text)
        { }
    }
}
