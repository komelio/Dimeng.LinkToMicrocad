using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.LinkToMicrocad
{
    public class Texture
    {
        public Texture()
        {
            Attributes = new List<TextureAttribute>();
        }
        public string ImageName { get; set; }
        public string Material { get; set; }
        public List<TextureAttribute> Attributes { get; private set; }
    }

    public struct TextureAttribute
    {
        public string Name;
        public bool IsDefault;
        public string Value;
    }
}
