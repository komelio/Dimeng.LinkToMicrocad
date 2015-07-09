using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.AutoCAD.Geometry;
using Dimeng.WoodEngine.Entities.Machinings;
using Dimeng.WoodEngine.Entities.Checks;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class HOLESMachiningToken : AssociativeToken
    {
        public HOLESMachiningToken(string token, string par1, string par2, string par3, string par4, string par5, string par6, string par7, string par8, string par9)
            : base(token, par1, par2, par3, par4, par5, par6, par7, par8, par9)
        {
            IsDrawOnly = false;
        }

        public override bool Valid(MachineTokenChecker check)
        {
            FaceNumber = check.FaceNumber(this.Token, 5, new int[] { 1, 2, 3, 4 }, this.Errors);

            PointsPosition = check.PointPositions(this.Par1, this.Errors);

            EdgeBoreDiameter = check.GetDoubleValue(this.Par2, @"HOLES/边孔直径", true, this.Errors);
            EdgeBoreDepth = check.GetDoubleValue(this.Par3, @"HOLES/边孔深度", true, this.Errors);

            ZValue = (string.IsNullOrEmpty(this.Par4)) ? Part.Thickness / 2
                : check.GetDoubleValue(this.Par4, @"Camlock/par4", true, this.Errors);

            FaceBoreDiameter = check.GetDoubleValue(this.Par5, @"HOLES/面孔直径", true, this.Errors);
            FaceBoreDepth = check.GetDoubleValue(this.Par6, @"HOLES/面孔深度", true, this.Errors);

            if (this.Errors.Count == 0)
            {
                return true;
            }
            else return false;
        }

        public List<double> PointsPosition { get; private set; }
        public double EdgeBoreDiameter { get; private set; }
        public double EdgeBoreDepth { get; private set; }
        public double FaceBoreDiameter { get; private set; }
        public double FaceBoreDepth { get; private set; }
        public double ZValue { get; private set; }

        //重写ToMachining方法，将Token转换为Machining
        public override void ToMachining(double tolerenceDist, Entities.ToolFile toolFile)
        {
            //step1 查找关联的板件
            //step2 建立一个临时的水平孔列表，把孔位坐标转换为水平机加工
            //setp3 把关联的垂直钻添加到关联的板件
            FindAssociatedFaces(0, tolerenceDist);

            PartFace pf = this.Part.GetPartFaceByNumber(FaceNumber);

            if (this.AssociatedPartFaces.Count != 0)//数量不为0，说明有关联的板件
            {
                List<HDrilling> TempHDrills = new List<HDrilling>();

                if (this.EdgeBoreDepth > 0 && this.EdgeBoreDiameter > 0)
                {
                    foreach (double d in this.PointsPosition)//遍历所欲的孔位坐标
                    {
                        HDrilling hdrill = new HDrilling(this.FaceNumber, this.EdgeBoreDiameter, this.EdgeBoreDepth, d, this.ZValue, Part,this);
                        TempHDrills.Add(hdrill);
                    }
                }

                if (this.FaceBoreDepth > 0 && this.FaceBoreDiameter > 0)
                {
                    foreach (PartFace f in this.AssociatedPartFaces)
                    {
                        if (!f.IsHorizontalFace)//如果关联的面是面5或面6
                        {
                            foreach (var hdrill in TempHDrills)
                            {
                                Point3d holeposition = hdrill.GetBorePosition();
                                holeposition = holeposition.TransformBy(Matrix3d.AlignCoordinateSystem(new Point3d(),
                                                                                                       Vector3d.XAxis,
                                                                                                       Vector3d.YAxis,
                                                                                                       Vector3d.ZAxis,
                                                                                                       hdrill.Part.MPPoint,
                                                                                                       hdrill.Part.MPXAxis,
                                                                                                       hdrill.Part.MPYAxis,
                                                                                                       hdrill.Part.MPZAxis));
                                holeposition = Math.MathHelper.GetRotatedAndMovedPoint(holeposition, Part.TXRotation, Part.YRotation, Part.ZRotation, Part.CenterVector);
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

                                VDrilling vdrill = new VDrilling(f.FaceNumber, dimx, dimy, this.FaceBoreDiameter, this.FaceBoreDepth, f.Part,this);
                                f.Part.VDrillings.Add(vdrill);
                            }
                        }
                        else//TODO:如果关联的面是水平面
                        {

                        }
                    }
                }

                this.Part.HDrillings.AddRange(TempHDrills);

            }
        }
    }
}
