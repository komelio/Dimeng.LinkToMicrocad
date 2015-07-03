using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities.Machinings
{
    public class Molding
    {
        public Molding(string dwgfile)
        {
            DWGFileName = dwgfile;
        }

        public string DWGFileName { get; private set; }
    }
}
