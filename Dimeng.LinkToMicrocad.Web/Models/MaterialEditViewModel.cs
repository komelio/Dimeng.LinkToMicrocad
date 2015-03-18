using Dimeng.LinkToMicrocad.Web.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dimeng.LinkToMicrocad.Web.Models
{
    public class MaterialEditViewModel
    {
        public MaterialEditViewModel(Material material)
        {
            this.Data = material;
        }

        public Material Data { get; set; }

        public Texture Texture { get; set; }
        public string Name { get; set; }
        public IEnumerable<Texture> TextureList { get; set; }
    }
}