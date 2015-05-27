using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Geometry;

namespace Dimeng.WoodEngine.Entities.Machinings
{
    public class Routing
    {
        public string ToolName {get;set;}
        public ToolComp ToolComp  {get;set;}
        public List<Point3d> Points { get; set; }
        public List<double> Bulges { get; set; }
        public List<double> FeedSpeeds { get; set; }
        public bool OnFace5 { get; set; }
        public Part Part { get; set; }

        public string DXFFile { get; set; }

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
        public bool HasChangedPartRectangleBorder
        {
            get
            {
                if (this.ToolComp == ToolComp.None)
                { return false; }

                double routeMaxthick = this.Points.Max(p => p.Z);
                if (routeMaxthick < Part.Thickness)
                {
                    return false;
                }

                //TODO:判断交点数量
                return true;
            }
        }
    }
}
