using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Geometry;
using Dimeng.WoodEngine.Entities;

namespace Dimeng.WoodEngine.Entities.Machinings
{
    public class Pocket
    {
        public Pocket()
        {
            this.Points = new List<Point3d>();
            this.Bulges = new List<double>();
        }
        public List<Point3d> Points { get; private set; }
        public List<double> Bulges { get; private set; }
        public double Depth { get; set; }
        public int FaceNumber { get; set; }

        public Part Part { get; set; }

        public bool OnTopFace
        {
            get
            {
                bool onTopFace = true;
                string mp = this.Part.MachinePoint.MP.Replace("M", "");
                if (mp == "1" || mp == "3" || mp == "5" || mp == "7")
                {
                    if (this.FaceNumber == 5)
                        onTopFace = false;
                }
                else
                {
                    if (!(this.FaceNumber == 5))
                        onTopFace = false;
                }

                return onTopFace;
            }
        }
    }
}
