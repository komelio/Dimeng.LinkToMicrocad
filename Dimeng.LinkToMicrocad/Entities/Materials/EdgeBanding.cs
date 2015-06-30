using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities
{
    public struct EdgeBanding
    {
        public static EdgeBanding Default()
        {
            return new EdgeBanding(string.Empty, 0, string.Empty);
        }

        public EdgeBanding(string name, double thick, string code)
        {
            this.Name = name;
            this.Thickness = thick;
            this.Code = code;
        }

        public double Thickness;
        public string Name;
        public string Code;

        public override string ToString()
        {
            return string.Format("{0}/{1}/{2}", Name, Thickness, Code);
        }
    }
}
