using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuoteExport.Entities
{
    public class Decco
    {
        public Decco()
        {
            IsExport = true;
        }

        public string Name { get; set; }
        public string Reference { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Depth { get; set; }
        public bool IsExport { get; set; }

        public string ImagePath { get; set; }
    }
}
