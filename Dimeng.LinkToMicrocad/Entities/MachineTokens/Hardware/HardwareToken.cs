using Autodesk.AutoCAD.Geometry;
using Dimeng.LinkToMicrocad.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class HardwareToken : AssociativeToken
    {
        public HardwareToken(string token, string p1, string p2, string p3, string p4, string p5, string p6, string p7, string p8, string p9)
            : base(token, p1, p2, p3, p4, p5, p6, p7, p8, p9)
        {
        }

        public virtual void ToMachining(ToolFile toolfile, IEnumerable<Part> combinedParts, Hardware hw)
        {

        }

        protected bool IsPointInPart(Point3d pt, Part p)
        {
            pt = pt.TransformBy(Matrix3d.AlignCoordinateSystem(
                                                               p.Point4,
                                                               p.MovedOrginXAxis,
                                                               p.MovedOrginYAxis,
                                                               p.MovedOrginZAxis,
                                                               Point3d.Origin,
                                                               Vector3d.XAxis,
                                                               Vector3d.YAxis,
                                                               Vector3d.ZAxis));

            Logger.GetLogger().Debug("Part is point in :" + p.PartName);
            Logger.GetLogger().Debug(pt.ToString());

            double z = System.Math.Round(pt.Z, 2);
            if (z > 0 || z < -p.Thickness)
                return false;

            if (!p.MachinePoint.IsRotated)
            {
                if (pt.X >= 0 && pt.X <= p.Length && pt.Y >= 0 && pt.Y <= p.Width)
                {
                    return true;
                }
                else return false;
            }
            else
            {
                if (pt.Y >= 0 && pt.Y <= p.Length && pt.X >= 0 && pt.X <= p.Width)
                {
                    return true;
                }
                else return false;
            }
        }
        protected int getFaceNumber(double z, double thick)
        {
            if (z > thick / 2)
            {
                return 6;
            }
            return 5;
        }
    }
}
