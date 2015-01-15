using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities.Machines.Tools
{
    public class Tool
    {
        public string Description { get; set; }
        public string ToolName { get; set; }
        public double Diameter { get; set; }
        public int FaceNumber { get; set; }
        public double FeedSpeed { get; set; }
        public double EntrySpeed { get; set; }
        public int Head { get; set; }
        public int PeckingNumber { get; set; }
        public ToolType ToolType { get; set; }
        public double XLocation { get; set; }
        public double YLocation { get; set; }
        public double ZLocation { get; set; }
        public double RotationSpeed { get; set; }
        public int RotationBit { get; set; }
        public double MinX { get; set; }
        public double MinY { get; set; }
        public double MinZ { get; set; }
        public double MaxX { get; set; }
        public double MaxY { get; set; }
        public double MaxZ { get; set; }
        public string ActualToolName { get; set; }

        public override string ToString()
        {
            return ToolName;
        }
    }
}
