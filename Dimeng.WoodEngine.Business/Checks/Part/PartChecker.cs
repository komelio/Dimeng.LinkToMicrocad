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
        string location;

        public PartChecker(IRange range, string location, List<ModelError> errors)
        {
            this.range = range;
            this.errors = errors;
            this.location = string.Format("{0}({1})", location, range.Row + 1);
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

        public void PartWarn(string message)
        {
            this.errors.Add(
                new ModelError(location, message, ErrorLevel.Warn)
                );
        }

        public void PartError(string message)
        {
            this.errors.Add(
                new ModelError(location, message, ErrorLevel.Error)
                );
        }
    }
}
