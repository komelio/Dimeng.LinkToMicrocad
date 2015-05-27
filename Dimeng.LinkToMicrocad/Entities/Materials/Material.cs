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
            Stocks = new List<Stock>();
        }
        public Material(string name, double thick)
            : this()
        {
            this.Name = name;
            this.Thickness = thick;
        }

        public string Name { get; set; }
        public double Thickness { get; set; }
        public List<Stock> Stocks { get; private set; }
        public Grain Grain { get; set; }
        public bool IsFake { get; set; }
        public override string ToString()
        {
            return this.Name;
        }

        public bool HasFitStock(double partLength, double partWidth)
        {
            return this.Stocks.Any(it =>
            {
                if (this.Grain == Grain.None)
                {
                    return (it.Length > partLength && it.Width > partWidth) || (it.Width > partLength && it.Length > partWidth);
                }
                else if (this.Grain == Grain.Length)
                {
                    return it.Length > partLength && it.Width > partWidth;
                }
                else
                {
                    return it.Width > partLength && it.Length > partWidth;
                }
            });
        }
    }
}
