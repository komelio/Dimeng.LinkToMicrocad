using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities
{
    public class BuyoutMaterial : Material
    {
        public BuyoutMaterial()
        {

        }
        public BuyoutMaterial(string name, double thick, string code)
            : this()
        {
            this.Name = name;
            this.Code = code;
            this.Thickness = thick;
        }
    }
}
