using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Business
{
    internal partial class PartChecker
    {
        internal int BasePoint()
        {
            return this.GetIntValue(range[0, 27].Text,
                "Part base point",
                true,
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8 },
                this.errors);
        }
    }
}
