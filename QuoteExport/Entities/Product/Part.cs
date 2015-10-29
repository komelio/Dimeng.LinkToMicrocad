using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuoteExport.Entities
{
    public class Part
    {
        public Part()
        {
            VDrillings = new List<VDrilling>();
            HDrillings = new List<HDrilling>();
        }
        public string PartName { get; set; }
        public int Qty { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }
        public double CutWidth { get; set; }
        public double CutLength { get; set; }
        public string Material { get; set; }
        public string MaterialCode { get; set; }
        public double Thickness { get; set; }
        public string EBW1 { get; set; }
        public double EBW1Thick { get; set; }
        public string EBW1Code { get; set; }
        public string EBW2 { get; set; }
        public string EBW2Code { get; set; }
        public double EBW2Thick { get; set; }
        public string EBL1 { get; set; }
        public double EBL1Thick { get; set; }
        public string EBL1Code { get; set; }
        public string EBL2 { get; set; }
        public double EBL2Thick { get; set; }
        public string EBL2Code { get; set; }
        public string Comment { get; set; }
        public string Comment2 { get; set; }
        public string Comment3 { get; set; }
        public string FileName { get; set; }
        public string Face6FileName { get; set; }
        public IProduct Parent { get; set; }

        public List<VDrilling> VDrillings { get; private set; }
        public List<HDrilling> HDrillings { get; private set; }
    }
}
