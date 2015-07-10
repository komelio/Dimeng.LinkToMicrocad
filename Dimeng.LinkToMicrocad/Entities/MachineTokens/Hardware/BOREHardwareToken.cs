using Autodesk.AutoCAD.Geometry;
using Dimeng.LinkToMicrocad.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class BOREHardwareToken : HardwareToken
    {
        public BOREHardwareToken(string token, string p1, string p2, string p3, string p4, string p5, string p6, string p7, string p8, string p9)
            : base(token, p1, p2, p3, p4, p5, p6, p7, p8, p9)
        {
        }

        public override bool Valid(Checks.MachineTokenChecker check)
        {
            this.StartX = string.IsNullOrEmpty(this.Par1) ? 0 :
                check.GetDoubleValue(this.Par1, "Hardware BORE/Par1/StartX ", false, this.Errors);
            this.StartY = string.IsNullOrEmpty(this.Par2) ? 0 :
                check.GetDoubleValue(this.Par2, "Hardware BORE/Par2/StartY", false, this.Errors);
            this.StartZ = string.IsNullOrEmpty(this.Par3) ? 0 :
                check.GetDoubleValue(this.Par3, "Hardware BORE/Par3/StartZ", false, this.Errors);
            this.EndX = string.IsNullOrEmpty(this.Par5) ? 0 :
                check.GetDoubleValue(this.Par5, "Hardware BORE/Par4/EndX", false, this.Errors);
            this.EndY = string.IsNullOrEmpty(this.Par6) ? 0 :
                check.GetDoubleValue(this.Par6, "Hardware BORE/Par5/EndY", false, this.Errors);
            this.Diameter = check.GetDoubleValue(this.Par4, "Hardware BORE/Par2/StartY", false, this.Errors);
            this.DistanceBetweenHoles = string.IsNullOrEmpty(this.Par7) ? 0 :
                check.GetDoubleValue(this.Par7, "Hardware BORE/Par2/StartY", false, this.Errors);
            this.DrillDepth = check.GetDoubleValue(this.Par8, "Hardware BORE/Par2/StartY", false, this.Errors);

            if (this.Errors.Count > 0)
            { return false; }
            else
            {
                return true;
            }
        }
        public double StartX { get; private set; }
        public double StartY { get; private set; }
        public double StartZ { get; private set; }
        public double EndX { get; private set; }
        public double EndY { get; private set; }
        public double Diameter { get; private set; }
        public double DistanceBetweenHoles { get; private set; }
        public double DrillDepth { get; private set; }

        public override void ToMachining(ToolFile toolfile, IEnumerable<Part> combinedParts, Hardware hw)
        {
            if (hw.AssociatedPart == null)
            {
                return;
            }

            Logger.GetLogger().Debug(string.Format("hardware bore token tomachining:{0}/{1}/{2}/{3}/{4}/{5}/{6}",
                this.StartX, this.StartY, this.StartZ, this.EndX, this.EndY, this.Diameter, this.DistanceBetweenHoles));

            //获取五金关于关联板件的点1的相对坐标
            Point3d pt = new Point3d(hw.TXOrigin, hw.TYOrigin, hw.TZOrigin);
            Logger.GetLogger().Debug("Point0 :" + pt.ToString());

            Vector3d zaxis = hw.AssociatedPart.MovedOrginZAxis.GetNormal();
            double angle = (!hw.OnTopFace) ? -hw.TAssociatedRotation : hw.TAssociatedRotation;
            Point3d ptZero = hw.AssociatedPart.GetPartPointPositionByNumber(4);
            Vector3d vx = hw.AssociatedPart.MovedOrginXAxis.GetNormal();
            Vector3d vy = hw.AssociatedPart.MovedOrginYAxis.GetNormal();

            pt = pt.TransformBy(Matrix3d.AlignCoordinateSystem(ptZero,
                                                               vx,
                                                               vy,
                                                               zaxis,
                                                               Point3d.Origin,
                                                               Vector3d.XAxis,
                                                               Vector3d.YAxis,
                                                               Vector3d.ZAxis
                                                               ));

            Logger.GetLogger().Debug("Point1 :" + pt.ToString());//五金相对关联板件点4的坐标

            double dist = System.Math.Sqrt(System.Math.Abs(StartX - EndX) * System.Math.Abs(StartX - EndX) +
                System.Math.Abs(StartY - EndY) * System.Math.Abs(StartY - EndY));
            Logger.GetLogger().Debug("Point dist:" + dist.ToString());

            int count = (this.DistanceBetweenHoles == 0) ? 0 : (int)System.Math.Floor(System.Math.Round(dist / this.DistanceBetweenHoles, 2));
            Logger.GetLogger().Debug("count :" + count.ToString());

            double lineAngle = System.Math.Atan2(EndY - StartY, EndX - StartX);
            Logger.GetLogger().Debug("lineAngle :" + lineAngle.ToString());

            for (int i = 0; i <= count; i++)
            {
                Point3d ptFirst = new Point3d(pt.X + StartX + (i * DistanceBetweenHoles) * System.Math.Cos(lineAngle),
                                              pt.Y + StartY + (i * DistanceBetweenHoles) * System.Math.Sin(lineAngle),
                                              hw.OnTopFace ? StartZ : -hw.AssociatedPart.Thickness - StartZ);
                Logger.GetLogger().Debug("Point2 :" + ptFirst.ToString());
                ptFirst = ptFirst.TransformBy(Matrix3d.Rotation((hw.OnTopFace ? -1 : 1) * angle / 180 * System.Math.PI, Vector3d.ZAxis, pt));
                ptFirst = ptFirst.TransformBy(Matrix3d.AlignCoordinateSystem(Point3d.Origin,
                                                                             Vector3d.XAxis,
                                                                             Vector3d.YAxis,
                                                                             Vector3d.ZAxis,
                                                                             ptZero,
                                                                             vx,
                                                                             vy,
                                                                             zaxis));
                Logger.GetLogger().Debug("Point3 :" + ptFirst.ToString());//转回世界坐标系的点坐标

                foreach (var p in combinedParts)
                {
                    if (this.IsPointInPart(ptFirst, p))
                    {
                        Logger.GetLogger().Debug("Associated Part :" + p.PartName);
                        ptFirst = ptFirst.TransformBy(Matrix3d.AlignCoordinateSystem(p.MovedMPPoint,
                                                                p.MovedMPXAxis,
                                                                p.MovedMPYAxis,
                                                                p.MovedMPZAxis,
                                                               Point3d.Origin,
                                                               Vector3d.XAxis,
                                                               Vector3d.YAxis,
                                                               Vector3d.ZAxis));
                        Logger.GetLogger().Debug("Point4 :" + ptFirst.ToString());

                        p.VDrillings.Add(
                            new Machinings.VDrilling(getFaceNumber(ptFirst.Z, p.Thickness),
                                ptFirst.X, ptFirst.Y, this.Diameter, this.DrillDepth, p, this));
                        break;
                    }
                }
            }
        }

        private int getFaceNumber(double z, double thick)
        {
            if (z > thick / 2)
            {
                return 6;
            }
            return 5;
        }
    }
}
