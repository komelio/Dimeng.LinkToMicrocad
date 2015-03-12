using Dimeng.LinkToMicrocad.Web.Domain.Abstract;
using Dimeng.LinkToMicrocad.Web.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dimeng.LinkToMicrocad.Web.Domain.Concrete
{
    public class EFMaterialRepository : IMaterialRepository
    {
        private EFDbContext context = new EFDbContext();

        public IEnumerable<Material> Materials
        {
            get
            {
                return context.Materials.Include("Texture");
            }
        }

        public int Add(Material material)
        {
            context.Materials.Add(material);
            context.SaveChanges();
            return material.Id;
        }

        public void AddRange(IEnumerable<Material> materials)
        {
            context.Materials.AddRange(materials);
            context.SaveChanges();
        }

        public void Clear()
        {
            context.Materials.RemoveRange(context.Materials);
            context.SaveChanges();
        }
    }
}
