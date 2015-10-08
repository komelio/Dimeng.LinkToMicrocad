using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuoteExport.Entities
{
    public class Moulding
    {
        public Moulding()
        {
            IsExport = true;
        }
        public string Description { get; set; }
        public double Length { get; set; }
        public string Comments { get; set; }
        public string Material { get; set; }
        public string Kind { get; set; }
        public bool IsExport { get; set; }
    }
}
