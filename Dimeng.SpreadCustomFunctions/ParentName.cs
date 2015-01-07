using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.SpreadCustomFunctions
{
    public class PARENTNAME : SpreadsheetGear.CustomFunctions.Function
    {
        public static readonly PARENTNAME pname = new PARENTNAME();
        public override void Evaluate(SpreadsheetGear.CustomFunctions.IArguments arguments, SpreadsheetGear.CustomFunctions.IValue result)
        {
            if (arguments.Count == 1)
            {
                string location = arguments.GetText(0);
                string parentname;
                string sheetname = arguments.CurrentWorksheet.ToString();
                if (sheetname.Length > 2 && sheetname.Substring(0, 3) == "[S]")
                    parentname = "L!";
                else if (sheetname.Length > 2 && sheetname.Substring(0, 3) == "[N]") parentname = "S!";
                else parentname = "L!";
                result.Text = parentname + location;
            }
            else
            {
                result.Error = SpreadsheetGear.ValueError.Value;
            }
        }
        private PARENTNAME()
            : base("PARENTNAME", SpreadsheetGear.CustomFunctions.Volatility.Invariant, SpreadsheetGear.CustomFunctions.ValueType.Text)
        { }
    }
}