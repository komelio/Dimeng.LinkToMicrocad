using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Business
{
    internal class SubassemblyChecker : Check
    {
        IRange range;
        List<ModelError> errors;
        string location;

        public SubassemblyChecker(IRange range, string location, List<ModelError> errors)
        {
            this.range = range;
            this.errors = errors;
            this.location = string.Format("{0}({1})", location, range.Row + 1);
        }

        internal string Name()
        {
            return range[0, 16].Text;
        }

        internal int Qty()
        {
            int value;
            if (int.TryParse(range[0, 17].Text, out value))
            {
                return value;
            }

            return 0;
        }

        internal double Width()
        {
            return GetDoubleValue(range[0, 18].Text, "Subassembly width", false, errors);
        }

        internal double Height()
        {
            return GetDoubleValue(range[0, 19].Text, "Subassembly height", false, errors);
        }

        internal double Depth()
        {
            return GetDoubleValue(range[0, 20].Text, "Subassembly depth", false, errors);
        }

        internal double XOrigin()
        {
            return GetDoubleValue(range[0, 29].Text, "Subassembly position_x", false, errors);
        }
        internal double YOrigin()
        {
            return GetDoubleValue(range[0, 30].Text, "Subassembly position_y", false, errors);
        }

        internal double ZOrigin()
        {
            return GetDoubleValue(range[0, 31].Text, "Subassembly position_z", false, errors);
        }

        internal double Rotation()
        {
            return GetDoubleValue(range[0, 31].Text, "Subassembly rotation", false, errors);
        }

        internal int LineNumber()
        {
            return range.Row + 1;
        }

        internal string Handle()
        {
            return range[0, 1].Text;
        }
    }
}
