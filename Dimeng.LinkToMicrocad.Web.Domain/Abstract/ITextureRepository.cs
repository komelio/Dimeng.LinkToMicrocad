using Dimeng.LinkToMicrocad.Web.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dimeng.LinkToMicrocad.Web.Domain.Abstract
{
    public interface ITextureRepository
    {
        IEnumerable<Texture> Textures { get; }
        int Add(Texture texture);
    }
}
