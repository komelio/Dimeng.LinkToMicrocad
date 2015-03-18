using Dimeng.LinkToMicrocad.Web.Domain.Abstract;
using Dimeng.LinkToMicrocad.Web.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dimeng.LinkToMicrocad.Web.Domain.Concrete
{
    public class EFTextureRepository : ITextureRepository
    {
        private EFDbContext context = new EFDbContext();

        public IEnumerable<Texture> Textures
        {
            get
            {
                return context.Textures;
            }
        }

        public int Add(Texture texture)
        {
            context.Textures.Add(texture);
            context.SaveChanges();
            return texture.TextureId;
        }

        public void ApplyModel(Texture texture)
        {
            context.Entry(texture).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            var texture = context.Textures.FirstOrDefault(it => it.TextureId == id);
            if (texture != null)
            {
                context.Textures.Remove(texture);
                context.SaveChanges();
            }
        }
    }
}
