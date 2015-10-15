using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuoteExport.Entities
{
    public class PauchiePart
    {
        public PauchiePart(Part part)
        {
            this.Part = part;
        }
        public Part Part { get; private set; }

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
        public string EdgeSKU1 { get; set; }
        public string EdgeColor1 { get; set; }
        public string EdgeSKU2 { get; set; }
        public string EdgeColor2 { get; set; }
        public string EdgeSKU3 { get; set; }
        public string EdgeColor3 { get; set; }
        public string EdgeSKU4 { get; set; }
        public string EdgeColor4 { get; set; }
        public int MachiningArea { get; set; }
        public int DrawerNumber { get; set; }
        public string FileName { get; set; }

        public object Face6FileName { get; set; }
    }
}
