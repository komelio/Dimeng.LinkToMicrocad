using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.SpreadCustomFunctions
{
    public class MyTitle : SpreadsheetGear.CustomFunctions.Function
    {
        public static readonly MyTitle myTitle = new MyTitle();

        public override void Evaluate(SpreadsheetGear.CustomFunctions.IArguments arguments, SpreadsheetGear.CustomFunctions.IValue result)
        {
            result.Text = arguments.CurrentWorksheet.Workbook.Name;
        }

        private MyTitle()
            : base("MyTitle", SpreadsheetGear.CustomFunctions.Volatility.Invariant, SpreadsheetGear.CustomFunctions.ValueType.Text)
        { }
    }
}
