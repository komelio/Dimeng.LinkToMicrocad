using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities
{
    public class Subassembly : IProduct
    {
        protected Subassembly()
        {
            this.Parts = new List<Part>();
            this.Hardwares = new List<Hardware>();
            this.Subassemblies = new List<Subassembly>();
        }
        public Subassembly(string name, int qty, double width, double height, double depth,
            double xorigin, double yorigin, double zorigin, double rotation, string handle, int lineNumber, IProduct parent)
            : this()
        {
            this.Description = name;
            this.Qty = qty;
            this.Width = width;
            this.Height = height;
            this.Depth = depth;
            this.XOrigin = xorigin;
            this.YOrigin = yorigin;
            this.ZOrigin = zorigin;
            this.Rotation = rotation;
            this.Handle = handle;
            this.LineNumber = lineNumber;
            this.Parent = parent;
        }
        public string Description { get; set; }
        public int Qty { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Depth { get; set; }
        public double XOrigin { get; set; }
        public double YOrigin { get; set; }
        public double ZOrigin { get; set; }
        public double Rotation { get; set; }
        public string Handle { get; set; }
        public int LineNumber { get; set; }

        public List<Part> Parts { get; private set; }
        public List<Hardware> Hardwares { get; private set; }
        public List<Subassembly> Subassemblies { get; private set; }
        public IProduct Parent { get; private set; }
    }
}
