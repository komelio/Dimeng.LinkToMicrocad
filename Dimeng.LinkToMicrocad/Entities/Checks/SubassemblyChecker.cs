using Dimeng.WoodEngine.Entities;
using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities.Checks
{
    public partial class SubassemblyChecker : Check
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

        public string Name()
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
            return GetDoubleValue(range[0, 18].Text, "Subassembly width", false, errors);
        }

        public double Height()
        {
            return GetDoubleValue(range[0, 19].Text, "Subassembly height", false, errors);
        }

        public double Depth()
        {
            return GetDoubleValue(range[0, 20].Text, "Subassembly depth", false, errors);
        }

        public double XOrigin()
        {
            return GetDoubleValue(range[0, 29].Text, "Subassembly position_x", false, errors);
        }
        public double YOrigin()
        {
            return GetDoubleValue(range[0, 30].Text, "Subassembly position_y", false, errors);
        }

        public double ZOrigin()
        {
            return GetDoubleValue(range[0, 31].Text, "Subassembly position_z", false, errors);
        }

        public double Rotation()
        {
            return GetDoubleValue(range[0, 34].Text, "Subassembly rotation", false, errors);
        }

        public int LineNumber()
        {
            return range.Row + 1;
        }

        public string Handle()
        {
            return range[0, 1].Text;
        }
    }
}
