using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities
{
    public class CutPartMaterial : Material
    {
        public CutPartMaterial()
        {
            Stocks = new List<Stock>();
        }

        public CutPartMaterial(string name, double thick, string code)
            :this()
        {
            this.Name = name;
            this.Code = code;
            this.Thickness = thick;
        }

        public List<Stock> Stocks { get; private set; }
        public Grain Grain { get; set; }
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
