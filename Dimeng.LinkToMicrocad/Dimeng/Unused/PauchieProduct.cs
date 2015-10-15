using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.LinkToMicrocad
{
    public class PauchieProduct
    {
        public PauchieProduct()
        {
            Hardwares = new List<PauchieHardware>();
            Parts = new List<PauchiePart>();
        }
        public string OrderNumber { get; set; }
        public int LineNumber { get; set; }
        public int Qty { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public List<PauchiePart> Parts { get; private set; }
        public List<PauchieHardware> Hardwares { get; private set; }
        public List<PauchieDrawerSubassembly> Drawers { get; private set; }

        public object Color { get; set; }
    }
}
