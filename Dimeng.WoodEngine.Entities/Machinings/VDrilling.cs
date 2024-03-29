﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities.Machinings
{
    public class VDrilling
    {
        public VDrilling(int facenumber, double dimX, double dimY, double diameter, double depth, Part p)
        {
            FaceNumber = facenumber;
            DimX = dimX;
            DimY = dimY;
            Diameter = diameter;
            Depth = depth;
            Part = p;
        }

        public int FaceNumber { get; private set; }
        public double DimX { get; private set; }
        public double DimY { get; private set; }
        public double Diameter { get; private set; }
        public double Depth { get; private set; }
        public Part Part { get; private set; }
    }
}
