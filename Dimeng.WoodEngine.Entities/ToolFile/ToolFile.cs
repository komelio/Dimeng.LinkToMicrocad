using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dimeng.WoodEngine.Entities.Machines.Tools;

namespace Dimeng.WoodEngine.Entities
{
    public class ToolFile
    {
        public ToolFile()
        {
            Tools = new List<Tool>();
        }

        public List<Tool> Tools { get; private set; }

        public Tool GetRouteToolByName(string name)
        {
            return Tools.Find(it => it.ToolName == name && it.ToolType == ToolType.Router);
        }
    }
}
