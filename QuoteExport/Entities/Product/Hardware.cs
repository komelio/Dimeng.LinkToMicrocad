using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuoteExport.Entities
{
    public class Hardware
    {
        public Hardware()
        {
            Comment = string.Empty;
            Comment2 = string.Empty;
            Comment3 = string.Empty;
        }
        public string Name { get; set; }
        public int Qty { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Depth { get; set; }
        public double XOrigin { get; set; }
        public double YOrigin { get; set; }
        public double ZOrigin { get; set; }
        public double AssociatedRotation { get; set; }
        public string Comment { get; set; }
        public string Comment2 { get; set; }
        public string Comment3 { get; set; }
        public string HardewareCode { get; set; }
    }
}
