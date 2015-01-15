using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities.Machinings
{
    public class Sawing
    {
        public string ToolName { get; set; }
        public double StartX { get; set; }
        public double StartY { get; set; }
        public double EndX { get; set; }
        public double EndY { get; set; }
        public double StartDepth { get; set; }
        public double EndDepth { get; set; }
        public bool OnFace5 { get; set; }
        public Part Part { get; set; }
        public ToolComp ToolComp { get; set; }
        public bool IsDrawOnly { get; set; }

        //TODO:这个应该和Routing的方法进行整合
        public bool OnTopFace
        {
            get
            {
                bool onTopFace = true;
                string mp = this.Part.MachinePoint.MP.Replace("M", "");
                if (mp == "1" || mp == "3" || mp == "5" || mp == "7")
                {
                    if (this.OnFace5)
                        onTopFace = false;
                }
                else
                {
                    if (!this.OnFace5)
                        onTopFace = false;
                }

                return onTopFace;
            }
        }
    }
}
