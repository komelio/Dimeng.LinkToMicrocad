using Dimeng.WoodEngine.Entities;
using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Business
{
    internal partial class PartChecker : Check
    {
        IRange range;
        List<ModelError> errors;

        public PartChecker(IRange range, List<ModelError> errors)
        {
            this.range = range;
            this.errors = errors;
        }

        public string PartName()
        {
            return range[0, 16].Text;
        }

        public int Qty()
        {
            int value;
            if (int.TryParse(range[0, 17].Text, out value))
            {
                return value;
            }

            return 0;
        }

        public double Width()
        {
            return GetDoubleValue(range[0, 18].Text, "Part Width", true, errors);
        }

        public double Length()
        {
            return GetDoubleValue(range[0, 19].Text, "Part Length", true, errors);
        }

      
    }
}
