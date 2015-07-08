using Autodesk.AutoCAD.Geometry;
using Dimeng.LinkToMicrocad.Logging;
using Dimeng.WoodEngine.Entities.Checks;
using Dimeng.WoodEngine.Entities.Machinings;
using Dimeng.WoodEngine.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class SLIDEMachiningToken : AssociativeToken
    {
        public SLIDEMachiningToken(string token, string par1, string par2, string par3, string par4, string par5, string par6, string par7, string par8, string par9)
            : base(token, par1, par2, par3, par4, par5, par6, par7, par8, par9)
        {

        }

        public override bool Valid(MachineTokenChecker check)
        {
            this.FaceNumber = check.FaceNumber(this.Token, 4, new int[] { 5, 6 });

            this.DistToBottom = check.GetDoubleValue(this.Par1, "SLIDE/Par1", false, check.Errors);
            this.DistToFirstBore = check.GetDoubleValue(this.Par2, "SLIDE/Par2", false, check.Errors);
            this.DistToSecondBore = check.GetDoubleValue(this.Par3, "SLIDE/Par3", false, check.Errors);
            this.AssociateGap = check.GetDoubleValue(this.Par8, "SLIDE/Par8", true, check.Errors);

            this.DistToThirdBore = string.IsNullOrEmpty(this.Par4.Trim()) ? 0 : check.GetDoubleValue(this.Par4, "SLIDE/Par4", true, check.Errors);

            this.DistToForthBore = string.IsNullOrEmpty(this.Par5.Trim()) ? 0 : check.GetDoubleValue(this.Par5, "SLIDE/Par5", true, check.Errors);

            this.DistToFifthBore = string.IsNullOrEmpty(this.Par6.Trim()) ? 0 : check.GetDoubleValue(this.Par6, "SLIDE/Par6", true, check.Errors);

            string[] values = this.Par7.Split('|');
            if (values.Length != 2)
            {
                check.Errors.Add(new ModelError("SLIDE/Par7", "没有用分隔符隔开", ErrorLevel.Error));
            }
            else
            {
                this.FaceBoreDepth = check.GetDoubleValue(values[0], "SLIDE/Par7", true, check.Errors);
                this.FaceBoreDiameter = check.GetDoubleValue(values[1], "SLIDE/Par7", true, check.Errors);
            }

            if (check.Errors.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public double DistToBottom { get; set; }
        public double DistToFirstBore { get; set; }
        public double DistToSecondBore { get; set; }
        public double DistToThirdBore { get; set; }
        public double DistToForthBore { get; set; }
        public double DistToFifthBore { get; set; }
        public double FaceBoreDiameter { get; set; }
        public double FaceBoreDepth { get; set; }
        public double AssociateGap { get; set; }

        public override void ToMachining(double AssociatedDist, ToolFile toolFile)
        {
            Logger.GetLogger().Info("Slide To machining:");
            Logger.GetLogger().Info(string.Format("Slide info:{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}",
                this.DistToBottom, this.DistToFirstBore, this.DistToSecondBore, this.DistToThirdBore, this.DistToForthBore,
                this.DistToFifthBore, this.FaceBoreDiameter, this.FaceBoreDepth, this.AssociateGap));

            int facenumber = getRealFacenumber();//由于MV的Slide指令是从面1所在的那个面来算的

            base.FindAssociatedFaces(this.AssociateGap, AssociatedDist);
            Logger.GetLogger().Info("Slide associated parts total:" + this.AssociatedPartFaces.Count.ToString());
            Logger.GetLogger().Info("Slide actual part face number:" + facenumber.ToString());

            PartFace pf = this.Part.GetPartFaceByNumber(facenumber);

            foreach (PartFace f in this.AssociatedPartFaces)
            {
                if (f.IsHorizontalFace)
                {
                    continue;
                }

                /*
                 * step1 先把孔按照机加工原点的方位换算
                 * step2 换算为被关联板件的位置
                 */

                List<Point3d> points = getPoints();
                for (int i = 0; i < points.Count; i++)
                {
                    Point3d holeposition = points[i];
                    holeposition = holeposition.TransformBy(Matrix3d.AlignCoordinateSystem(new Point3d(),
                                                                                                      Vector3d.XAxis,
                                                                                                      Vector3d.YAxis,
                                                                                                      Vector3d.ZAxis,
                                                                                                      this.Part.MPPoint,
                                                                                                      this.Part.MPXAxis,
                                                                                                      this.Part.MPYAxis,
                                                                                                      this.Part.MPZAxis));
                    holeposition = MathHelper.GetRotatedAndMovedPoint(holeposition, this.Part.TXRotation, this.Part.YRotation, this.Part.ZRotation, Part.CenterVector);
                    holeposition = holeposition.TransformBy(Matrix3d.AlignCoordinateSystem(f.Part.MovedMPPoint,
                                                                                           f.Part.MovedMPXAxis,
                                                                                           f.Part.MovedMPYAxis,
                                                                                           f.Part.MovedMPZAxis,
                                                                                           new Point3d(),
                                                                                           Vector3d.XAxis,
                                                                                           Vector3d.YAxis,
                                                                                           Vector3d.ZAxis));
                    double dimx = holeposition.X;
                    double dimy = holeposition.Y;

                    VDrilling vdrill = new VDrilling(f.FaceNumber, dimx, dimy, this.FaceBoreDiameter, this.FaceBoreDepth, f.Part, this);
                    f.Part.VDrillings.Add(vdrill);
                }
            }
        }

        private List<Point3d> getPoints()
        {
            List<Point3d> points = new List<Point3d>();

            points.Add(new Point3d(DistToFirstBore, DistToBottom, 0));
            points.Add(new Point3d(DistToSecondBore, DistToBottom, 0));

            if (DistToThirdBore > 0)
            {
                points.Add(new Point3d(DistToThirdBore, DistToBottom, 0));
            }

            if (DistToForthBore > 0)
            {
                points.Add(new Point3d(DistToForthBore, DistToBottom, 0));
            }

            if (DistToFifthBore > 0)
            {
                points.Add(new Point3d(DistToFifthBore, DistToBottom, 0));
            }

            return points;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int getRealFacenumber()
        {
            if (Part.MachinePoint.MP == "1" || Part.MachinePoint.MP == "1M"
                || Part.MachinePoint.MP == "3" || Part.MachinePoint.MP == "3M"
                 || Part.MachinePoint.MP == "5" || Part.MachinePoint.MP == "5M"
                 || Part.MachinePoint.MP == "7" || Part.MachinePoint.MP == "7M")
            {
                if (FaceNumber == 5)
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
                if (FaceNumber == 5)
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
