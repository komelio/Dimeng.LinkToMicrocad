using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuoteExport.Entities
{
    public class Product
    {
        public string Description { get; set; }
        public string Handle { get; set; }
        public string ItemNumber { get; set; }
        public int Qty { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Depth { get; set; }
        public string Comments { get; set; }
    }
}
