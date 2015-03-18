using Dimeng.LinkToMicrocad.Web.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dimeng.LinkToMicrocad.Web.Domain.Abstract
{
    public interface IProductRepository
    {
        IEnumerable<Product> Products { get; }

        int Add(Product product);

        void AddRange(IEnumerable<Product> products);

        void Clear();

        void ApplyModel(Product product);
    }
}
