using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuoteExport.Entities
{
    public class Subassembly : IProduct
    {
        public Subassembly()
        {
            Parts = new List<Part>();
            Hardwares = new List<Hardware>();
            Subassemblies = new List<Subassembly>();
        }

        public string Description { get; set; }
        public int Qty { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Depth { get; set; }
        public string Comment { get; set; }
        public string Comment2 { get; set; }
        public string Comment3 { get; set; }

        public List<Part> Parts { get; set; }
        public List<Hardware> Hardwares { get; set; }
        public List<Subassembly> Subassemblies { get; set; }
    }
}
