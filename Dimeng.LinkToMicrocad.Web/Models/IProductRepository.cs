using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dimeng.LinkToMicrocad.Web.Models
{
    public interface IProductRepository
    {
        IEnumerable<MVProduct> Products { get; }
    }
}