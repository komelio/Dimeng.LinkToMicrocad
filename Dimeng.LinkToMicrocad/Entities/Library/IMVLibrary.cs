using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities
{
    public interface IMVLibrary
    {
        string Library { get; }
        string MicrovellumData { get; }
        string Subassemblies { get; }
        string Template { get; }
        string Toolfiles { get; }
    }
}
