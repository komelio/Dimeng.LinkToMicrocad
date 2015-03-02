using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dimeng.LinkToMicrocad.Web.Domain.Entities
{
    public class Release
    {
        public DateTime CreateTime { get; set; }
        public string MainVersionNumber { get; set; }
        public string AddonVersionNumber { get; set; }
        public string Description { get; set; }
        public List<Product> Products { get; set; }
        public List<Material> Materials { get; set; }
    }
}
