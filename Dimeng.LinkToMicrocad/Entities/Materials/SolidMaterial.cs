using Dimeng.WoodEngine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities
{
    public class SolidMaterial : Material
    {
        public SolidMaterial()
        {

        }
        public SolidMaterial(string name, double thick, string code)
            : this()
        {
            this.Name = name;
            this.Code = code;
            this.Thickness = thick;
        }
    }
}
