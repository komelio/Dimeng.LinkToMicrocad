using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.LinkToMicrocad
{
    public class PauchiePart
    {
        public int Index { get; set; }
        public string Color { get; set; }
        public string PartName { get; set; }
        public string Category { get; set; }
        public string SKU { get; set; }
        public int Qty { get; set; }
        public string Model { get; set; }//造型
        public double CutLength { get; set; }
        public double CutWidth { get; set; }
        public double CutThickness { get; set; }
        public double CutSquare { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Thickness { get; set; }
        public double Square { get; set; }
        public string EdgeSKU { get; set; }
        public string EdgeColor { get; set; }
        public int MachiningArea { get; set; }
        public int DrawerNumber { get; set; }
        public string FileName { get; set; }
    }
}
