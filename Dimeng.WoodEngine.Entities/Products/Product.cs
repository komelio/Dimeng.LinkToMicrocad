using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities
{
    public class Product : IProduct
    {
        public Product()
        {
            this.Parts = new List<Part>();
            this.CombinedHardwares = new List<Hardware>();
            this.CombinedParts = new List<Part>();
            this.Subassemblies = new List<Subassembly>();
            this.Hardwares = new List<Hardware>();
        }

        public string Description { get; set; }
        public string Handle { get; set; }
        public string ItemNumber { get; set; }
        public int Qty { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Depth { get; set; }
        public string Comments { get; set; }
        public string MatFile { get; set; }//规格组名称
        public string FileName { get; set; }
        public string ReleaseNumber { get; set; }
        public string ReleaseDate { get; set; }
        public string Parent1 { get; set; }
        public string Parent2 { get; set; }
        public string Parent3 { get; set; }
        public string Parent4 { get; set; }
        public string Parent5 { get; set; }
        public string Parent6 { get; set; }
        public string Parent7 { get; set; }
        public string Parent8 { get; set; }
        public string Parent9 { get; set; }

        public Project Project { get; set; }

        public string GetProductCutxFileName()
        {
            return string.Format("{0}\\{1}", this.Project.JobPath, FileName);
        }

        public void ClearData()
        {
            this.Parts.Clear();
            this.Subassemblies.Clear();
            this.Hardwares.Clear();
            this.CombinedParts.Clear();
            this.CombinedHardwares.Clear();
        }

        public List<Part> Parts { get; private set; }
        public List<Part> CombinedParts { get; private set; }
        public List<Hardware> Hardwares { get; private set; }
        public List<Hardware> CombinedHardwares { get; private set; }
        public List<Subassembly> Subassemblies { get; private set; }
    }
}
