using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities
{
    public class Subassembly:IProduct
    {
        public List<Part> Parts
        {
            get { throw new NotImplementedException(); }
        }

        public List<Hardware> Hardwares
        {
            get { throw new NotImplementedException(); }
        }

        public List<Subassembly> Subassemblies
        {
            get { throw new NotImplementedException(); }
        }
    }
}
