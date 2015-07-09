using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class PLINEHardwareToken : HardwareToken
    {
        public PLINEHardwareToken(string token, string p1, string p2, string p3, string p4, string p5, string p6, string p7, string p8, string p9)
            : base(token, p1, p2, p3, p4, p5, p6, p7, p8, p9)
        {
        }
        public List<Point3d> Points { get; private set;}
        public List<double> Bulges { get; private set; }
        public List<double> FeedSpeeds { get; private set; }
        public double Offset { get; private set; }
        public string ProfileName { get; private set; }
        public string ToolName { get; private set; }
        public ToolComp ToolComp { get; private set; }
        public double RouteDepth { get; private set; }
    }
}
