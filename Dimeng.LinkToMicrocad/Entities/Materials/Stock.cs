using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities
{
    public struct Stock
    {
        public static Stock Default
        {
            get { return new Stock(); }
        }

        public Stock(double l, double w, int q, double price,
            double topTrim, double rightTrim, double bottomTrim, double leftTrim)
        {
            Length = l;
            Width = w;
            Qty = q;
            Price = price;
            TopTrimValue = topTrim;
            BottomTrimValue = bottomTrim;
            LeftTrimValue = leftTrim;
            RightTrimValue = rightTrim;
        }

        public double Length;
        public double Width;
        public int Qty;
        public double Price;
        public double TopTrimValue;
        public double BottomTrimValue;
        public double LeftTrimValue;
        public double RightTrimValue;
    }
}
