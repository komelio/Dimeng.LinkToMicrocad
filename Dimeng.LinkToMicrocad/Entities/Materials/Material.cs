using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities
{
    public class Material
    {
        public Material()
        {

        }
        public Material(string name, double thick, string code)
        {
            this.Name = name;
            this.Code = code;
            this.Thickness = thick;
        }

        public string Name { get; set; }
        public string Code { get; set; }
        public double Thickness { get; set; }
        
        public bool IsFake { get; set; }
        public override string ToString()
        {
            return this.Name;
        }
    }
}
