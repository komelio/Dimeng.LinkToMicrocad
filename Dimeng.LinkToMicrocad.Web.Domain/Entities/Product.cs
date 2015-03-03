using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dimeng.LinkToMicrocad.Web.Domain.Entities
{
    public class Product
    {
        public Product()
        {
            Width = 400;
            Height = 400;
            Depth = 400;
            Elevation = 0;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Depth { get; set; }
        public double Elevation { get; set; }
    }
}
