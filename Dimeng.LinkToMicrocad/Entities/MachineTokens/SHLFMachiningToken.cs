﻿using Autodesk.AutoCAD.Geometry;
using Dimeng.WoodEngine.Entities.Checks;
using Dimeng.WoodEngine.Entities.Machinings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class SHLFMachiningToken : AssociativeToken
    {
        public SHLFMachiningToken(string token, string par1, string par2, string par3, string par4, string par5, string par6, string par7, string par8, string par9)
            : base(token, par1, par2, par3, par4, par5, par6, par7, par8, par9)
        {

        }

        public override bool Valid(MachineTokenChecker check)
        {
            this.FaceNumber = check.FaceNumber(this.Token, 4, new int[] { 1, 2, 3, 4 });

            DistToBottom = check.GetDoubleValue(this.Par1, "关联的层板孔（类型1）/到底部距离", true, check.Errors);
            DistToFirstHole = check.GetDoubleValue(this.Par2, "关联的层板孔（类型1）/至第1排孔的距离", false, check.Errors);
            FaceHoleDepth = check.GetDoubleValue(this.Par3, "关联的层板孔（类型1）/面孔深度", true, check.Errors);
            DistToTop = check.GetDoubleValue(this.Par4, "关联的层板孔（类型1）/至顶部的距离", true, check.Errors);
            DistToSecondHole = check.GetDoubleValue(this.Par5, "关联的层板孔（类型1）/至第二排孔的距离", false, check.Errors);
            FaceHoleDiameter = check.GetDoubleValue(this.Par6, "关联的层板孔（类型1）/面孔直径", true, check.Errors);
            DistBetweenTwoHoles = string.IsNullOrEmpty(this.Par7) ? 32 : check.GetDoubleValue(this.Par7, "关联的层板孔（类型1）/面孔直径", true, check.Errors);
            GapDist = check.GetDoubleValue(Par8, "关联的层板孔（类型1）/两孔距离", true, check.Errors);
            IsDrillFromOpposite = check.GetBoolValue(this.Par9, "关联的层板孔（类型1）/反方向", false, false, check.Errors);//反向的


            if (check.Errors.Count == 0)
            {
                return true;
            }
            else return false;
        }

        public double DistToBottom { get; set; }
        public double DistToFirstHole { get; set; }
        public double FaceHoleDepth { get; set; }
        public double DistToTop { get; set; }
        public double FaceHoleDiameter { get; set; }
        public double DistToSecondHole { get; set; }
        public double DistBetweenTwoHoles { get; set; }
        public double GapDist { get; set; }
        public bool IsDrillFromOpposite { get; private set; }

        public override void ToMachining(double AssociatedDist, ToolFile toolFile)
        {
            base.FindAssociatedFaces(this.GapDist, AssociatedDist);

            PartFace pf = this.Part.GetPartFaceByNumber(FaceNumber);

            if (this.AssociatedPartFaces.Count != 0)//数量不为0，说明有关联的板件
            {
                double totolDist = DistToBottom + DistToTop;
                int number = (int)(totolDist / DistBetweenTwoHoles);
                for (int i = 0; i < number; i++)
                {
                    foreach (PartFace f in this.AssociatedPartFaces)
                    {
                        if (f.IsHorizontalFace)//不对水平面处理
                            continue;


                        //TODO:关于生成的孔的排列
                        /*
                         * 1、无论层板如何旋转，计算上下时总是按未位移旋转时的下平面为基准
                         * 2、排列时，无论本身如何旋转，在加工板件上总是竖直平行于板件的一边
                         * 3、平行的难点：是平行于长边，还是平行于短边？
                         * 
                         */


                        double firstPointX = GetFirstPointX();
                        double firstPointY = GetFirstPointY();
                        double firstPointZ = DistToBottom - i * DistBetweenTwoHoles;
                        //firstPointZ = -Part.Thickness - DistToBottom + i * DistBetweenTwoHoles;
                        Point3d holeposition = new Point3d(firstPointX, firstPointY, firstPointZ);

                        holeposition = holeposition.TransformBy(Matrix3d.AlignCoordinateSystem(new Point3d(),
                                                                                                           Vector3d.XAxis,
                                                                                                           Vector3d.YAxis,
                                                                                                           Vector3d.ZAxis,
                                                                                                           Part.MPPoint,
                                                                                                           Part.MPXAxis,
                                                                                                           Part.MPYAxis,
                                                                                                           Part.MPZAxis));
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

                        VDrilling vdrill = new VDrilling(f.FaceNumber, dimx, dimy, this.FaceHoleDiameter, this.FaceHoleDepth, f.Part, this);
                        f.Part.VDrillings.Add(vdrill);
                    }
                }
            }
        }

        private double GetFirstPointX()
        {
            //板件是否旋转过
            if (!this.Part.MachinePoint.IsRotated)
            {
                if (FaceNumber == 1 || FaceNumber == 2)
                    return DistToFirstHole;
                else if (FaceNumber == 3)
                    return 0;
                else return Part.Length;
            }
            else
            {
                if (FaceNumber == 1 || FaceNumber == 2)
                    return DistToFirstHole;
                else if (FaceNumber == 3)
                    return 0;
                else return Part.Width;
            }
        }
        private double GetFirstPointY()
        {
            //板件是否旋转过
            if (!this.Part.MachinePoint.IsRotated)
            {
                if (FaceNumber == 1)
                    return 0;
                else if (FaceNumber == 3 || FaceNumber == 4)
                    return DistToFirstHole;
                else return Part.Width;
            }
            else
            {
                if (FaceNumber == 1)
                    return 0;
                else if (FaceNumber == 3 || FaceNumber == 4)
                    return DistToFirstHole;
                else return Part.Length;
            }
        }
    }
}
