using Autodesk.AutoCAD.Geometry;
using Dimeng.LinkToMicrocad.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class CUTOUTHardwareToken : HardwareToken
    {
        public CUTOUTHardwareToken(string token, string p1, string p2, string p3, string p4, string p5, string p6, string p7, string p8, string p9)
            : base(token, p1, p2, p3, p4, p5, p6, p7, p8, p9)
        {
        }
        public double StartX { get; private set; }
        public double StartY { get; private set; }
        public double StartZ { get; private set; }
        public double EndX { get; private set; }
        public double EndY { get; private set; }
        public bool IsPocket { get; private set; }
        public string ToolName { get; private set; }
        public double RouteDepth { get; private set; }

        public override bool Valid(Checks.MachineTokenChecker check)
        {
            this.StartX = string.IsNullOrEmpty(this.Par1) ? 0 :
                check.GetDoubleValue(this.Par1, "Hardware CUTOUT/Par1/StartX ", false, this.Errors);
            this.StartY = string.IsNullOrEmpty(this.Par2) ? 0 :
                check.GetDoubleValue(this.Par2, "Hardware CUTOUT/Par2/StartY", false, this.Errors);
            this.StartZ = string.IsNullOrEmpty(this.Par3) ? 0 :
                check.GetDoubleValue(this.Par3, "Hardware CUTOUT/Par3/StartZ", false, this.Errors);
            this.EndX = string.IsNullOrEmpty(this.Par5) ? 0 :
                check.GetDoubleValue(this.Par4, "Hardware CUTOUT/Par4/EndX", false, this.Errors);
            this.EndY = string.IsNullOrEmpty(this.Par6) ? 0 :
                check.GetDoubleValue(this.Par5, "Hardware CUTOUT/Par5/EndY", false, this.Errors);
            this.IsPocket = check.GetBoolValue(this.Par6, "Hardware CUTOUT/Par6/IsPocket", false, false, this.Errors);
            this.ToolName = check.ToolName(this.Par7, "Hardware CUTOUT/Par7/ToolName", this.Errors);
            this.RouteDepth = check.GetDoubleValue(this.Par8, "Hardware CUTOUT/Par8/ROUTEDEPTH", true, this.Errors);

            if (this.Errors.Count > 0)
            { return false; }
            else
            {
                return true;
            }
        }

        public override void ToMachining(ToolFile toolfile, IEnumerable<Part> combinedParts, Hardware hw)
        {
            if (hw.AssociatedPart == null)
            {
                return;
            }

            Logger.GetLogger().Debug(string.Format("hardware cutout token tomachining:{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}",
                this.StartX, this.StartY, this.StartZ, this.EndX, this.EndY, this.IsPocket, this.ToolName,this.RouteDepth));

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

   
                        break;
                    }
                }


        }
    }


}
