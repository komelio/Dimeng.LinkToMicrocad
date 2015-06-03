using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dimeng.LinkToMicrocad.Web.Domain.Entities
{
    public class Release
    {
        public int Id { get; set; }
        public DateTime CreateTime { get; set; }
        public string Description { get; set; }
    }
}
