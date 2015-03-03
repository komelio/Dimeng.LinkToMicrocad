using Dimeng.LinkToMicrocad.Web.Domain.Abstract;
using Dimeng.LinkToMicrocad.Web.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dimeng.LinkToMicrocad.Web.Domain.Concrete
{
    public class EFProductRepository : IProductRepository
    {
        private EFDbContext context = new EFDbContext();
        public IEnumerable<Entities.Product> Products
        {
            get { return context.Products; }
        }

        public int Add(Product product)
        {
            context.Products.Add(product);
            context.SaveChanges();

            return product.Id;
        }

        public void AddRange(IEnumerable<Product> products)
        {
            context.Products.AddRange(products);
            context.SaveChanges();
        }

        public void Clear()
        {
            context.Products.RemoveRange(context.Products);
            context.SaveChanges();
        }
    }
}
