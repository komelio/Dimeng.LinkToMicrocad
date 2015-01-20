using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities
{
    public interface IProduct
    {
        double Width { get; }
        double Height { get; }
        double Depth { get; }
        List<Part> Parts { get; }
        List<Hardware> Hardwares { get; }
        List<Subassembly> Subassemblies { get; }
    }
}
