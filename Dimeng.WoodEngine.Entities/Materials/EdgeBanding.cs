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
            return new EdgeBanding();
        }

        public EdgeBanding(string name,double thick)
        {
            this.Name = name;
            this.Thickness = thick;
        }

        public double Thickness;
        public string Name;

        public override string ToString()
        {
            return Name;
        }
    }
}
