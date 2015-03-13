using Dimeng.LinkToMicrocad.Web.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dimeng.LinkToMicrocad.Web.Models
{
    public class TextureListViewModel
    {
        public IEnumerable<Texture> Textures { get; set; }

        public PagingInfo PagingInfo { get; set; }
    }
}