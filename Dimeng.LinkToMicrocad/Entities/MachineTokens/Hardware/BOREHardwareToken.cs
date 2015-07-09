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
                check.GetDoubleValue(this.Par5, "Hardware BORE/Par2/StartY", false, this.Errors);
            this.EndY = string.IsNullOrEmpty(this.Par6) ? 0 :
                check.GetDoubleValue(this.Par6, "Hardware BORE/Par2/StartY", false, this.Errors);
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

            //获取五金关于关联板件的点1的相对坐标
            Point3d pt = new Point3d(hw.TXOrigin, hw.TYOrigin, hw.TZOrigin);
            Logger.GetLogger().Warn("Point0 :" + pt.ToString());

            Vector3d zaxis = (hw.OnTopFace) ? hw.AssociatedPart.MovedOrginZAxis : -hw.AssociatedPart.MovedOrginZAxis;
            zaxis = zaxis.GetNormal();
            double angle = (!hw.OnTopFace) ? -hw.TAssociatedRotation : hw.TAssociatedRotation;
            Point3d ptZero = hw.AssociatedPart.GetPartPointPositionByNumber(3);
            Vector3d vx = hw.AssociatedPart.MovedOrginXAxis.GetNormal();
            Vector3d vy = hw.AssociatedPart.MovedOrginYAxis.GetNormal();

            Logger.GetLogger().Info(ptZero.ToString());
            Logger.GetLogger().Info(vx.ToString());
            Logger.GetLogger().Info(vy.ToString());
            Logger.GetLogger().Info(zaxis.ToString());

            pt = pt.TransformBy(Matrix3d.AlignCoordinateSystem(ptZero,
                                                               vx,
                                                               vy,
                                                               zaxis,
                                                               Point3d.Origin,
                                                               Vector3d.XAxis,
                                                               Vector3d.YAxis,
                                                               Vector3d.ZAxis
                                                               ));

            Logger.GetLogger().Info("Point1 :" + pt.ToString());

            //先假设是发生在关联板件上的
            Point3d drillPt = new Point3d(pt.X + StartX, pt.Y + StartY, 0);
            Logger.GetLogger().Info("Point2 :" + drillPt.ToString());

            drillPt = drillPt.TransformBy(
                Matrix3d.Rotation(angle / 180 * System.Math.PI, Vector3d.ZAxis, pt));
            Logger.GetLogger().Info("Point3 :" + drillPt.ToString());

            //再把点转回到以机加工原点的坐标上
            Logger.GetLogger().Info(hw.AssociatedPart.MovedMPPoint.ToString());
            Logger.GetLogger().Info(hw.AssociatedPart.MovedMPXAxis.ToString());
            Logger.GetLogger().Info(hw.AssociatedPart.MovedMPYAxis.ToString());
            Logger.GetLogger().Info(hw.AssociatedPart.MovedMPZAxis.ToString());
            drillPt = drillPt.TransformBy(Matrix3d.AlignCoordinateSystem(Point3d.Origin,
                                                                         Vector3d.XAxis,
                                                                         Vector3d.YAxis,
                                                                         Vector3d.ZAxis,
                                                                         hw.AssociatedPart.GetPartPointPositionByNumber(3),
                                                                         hw.AssociatedPart.MovedOrginXAxis,
                                                                         hw.AssociatedPart.MovedOrginYAxis,
                                                                         zaxis
                                                                         ));
            Logger.GetLogger().Info("Point4 :" + drillPt.ToString());

            drillPt = drillPt.TransformBy(Matrix3d.AlignCoordinateSystem(hw.AssociatedPart.MovedMPPoint,
                                                                hw.AssociatedPart.MovedMPXAxis,
                                                                hw.AssociatedPart.MovedMPYAxis,
                                                                hw.AssociatedPart.MovedMPZAxis,
                                                               Point3d.Origin,
                                                               Vector3d.XAxis,
                                                               Vector3d.YAxis,
                                                               Vector3d.ZAxis));
            Logger.GetLogger().Info("Point5 :" + drillPt.ToString());
            hw.AssociatedPart.VDrillings.Add(
                new Machinings.VDrilling(getFaceNumber(hw.OnTopFace, hw.AssociatedPart.MachinePoint.MP),
                    drillPt.X, drillPt.Y, this.Diameter, this.DrillDepth, hw.AssociatedPart, this));

            //foreach (var part in combinedParts)//判断起点是否在某个板件的内部
            //{
            //    if (base.IsPointInPart(pt, part))
            //    {
            //        Point3d drillPt = new Point3d(pt.X+StartX,pt.Y;


            //        int faceNumber = getFaceNumber(hw.OnTopFace, part.MachinePoint.MP);
            //        double x = S
            //        part.VDrillings.Add(new Machinings.VDrilling(faceNumber, StartX, StartY, Diameter, DrillDepth, part, this));

            //        Logger.GetLogger().Info(
            //            string.Format("Add hardware bore machining:Face{0}/X{1}/Y{2}/D{3}/Dia{4}", faceNumber, StartX, StartY, DrillDepth, Diameter));
            //    }
            //}

        }

        private int getFaceNumber(bool onTopFace, string machinePoint)
        {
            machinePoint = machinePoint.Replace("M", "");
            if (machinePoint == "1" || machinePoint == "3" || machinePoint == "5" || machinePoint == "5" || machinePoint == "7")
            {
                if (onTopFace)
                {
                    return 6;
                }
                else
                {
                    return 5;
                }
            }
            else
            {
                if (onTopFace)
                {
                    return 5;
                }
                else
                {
                    return 6;
                }
            }
        }
    }
}
