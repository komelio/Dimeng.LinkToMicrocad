using Dimeng.LinkToMicrocad.Web.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dimeng.LinkToMicrocad.Web.Domain.Concrete
{
    public class EFProductCategoryRepository : IProductCategoryRepository
    {
        public IEnumerable<Entities.ProductCategory> Categories
        {
            get { throw new NotImplementedException(); }
        }

        public int Add(Entities.ProductCategory category)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void ApplyModel(Entities.ProductCategory category)
        {
            throw new NotImplementedException();
        }
    }
}
