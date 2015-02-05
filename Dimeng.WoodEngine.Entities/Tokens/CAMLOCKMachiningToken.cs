using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Geometry;
using Dimeng.WoodEngine.Entities.Machinings;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class CAMLOCKMachiningToken : BaseToken
    {
        public CAMLOCKMachiningToken(string token, string par1, string par2, string par3,
            string par4, string par5, string par6, string par7, string par8, string par9)
            : base(token, par1, par2, par3, par4, par5, par6, par7, par8, par9)
        {
            IsDrawOnly = false;
        }

        public override bool Valid()
        {
            //base.faceNumberChecker(this.Token, 7, new int[] { 1, 2, 3, 4 });
            //PointsPosition = base.pointsChecker(this.Par1);
            //EdgeBoreDiameter = base.DoubleChecker(this.Par2, @"Camlock/边孔直径", true);
            //EdgeBoreDepth = base.DoubleChecker(this.Par3, @"Camlock/边孔深度", true);
            //ZValue = (string.IsNullOrEmpty(this.Par4)) ? Part.Thickness / 2 : base.DoubleChecker(this.Par4, @"Camlock/par4", true);
            //diameterChecker(this.Par5);
            //depthChecker(this.Par6);
            //Backset = base.DoubleChecker(this.Par7, @"Camlock/par7", false);
            //camlockFaceChecker();
            //this.DrillFromOppsiteFace = base.BoolChecker(this.Par9, @"Camlock/par9", false, false);
            //ListCamVBoreXY = GetCamVBoreXYList();

            //return this.IsValid;

            return true;
        }

        public List<double> PointsPosition { get; private set; }
        public double EdgeBoreDiameter { get; private set; }
        public double EdgeBoreDepth { get; private set; }
        public double FaceBoreDiameter { get; private set; }
        public double FaceBoreDepth { get; private set; }
        public int CamFaceNumber { get; private set; }
        public double CamFaceBoreDiameter { get; private set; }
        public double CamFaceBoreDepth { get; private set; }
        public double Backset { get; private set; }
        public double ZValue { get; private set; }
        public bool DrillFromOppsiteFace { get; private set; }

        public List<StructXY> ListCamVBoreXY { get; private set; }

        private List<StructXY> GetCamVBoreXYList()
        {
            List<StructXY> Pairs = new List<StructXY>();

            if (FaceNumber == 1)
            {
                PointsPosition.ForEach(d => Pairs.Add(new StructXY() { X = d, Y = Backset }));
            }
            else if (FaceNumber == 2)
            {
                PointsPosition.ForEach(d => Pairs.Add(new StructXY() { X = d, Y = Part.Width - Backset }));
            }
            else if (FaceNumber == 3)
            {
                PointsPosition.ForEach(d => Pairs.Add(new StructXY() { X = Backset, Y = d }));
            }
            else if (FaceNumber == 4)
            {
                PointsPosition.ForEach(d => Pairs.Add(new StructXY() { X = Part.Length - Backset, Y = d }));
            }

            return Pairs;
        }

        public override void ToMachining(double associateDist, Entities.ToolFile toolFile)
        {
            //FindAssociatedFaces(associateDist);

            PartFace pf = this.Part.GetPartFaceByNumber(FaceNumber);
            if (pf.AssociatedPartFaces.Count != 0)//数量不为0，说明有关联的板件
            {
                List<HDrilling> TempHDrills = new List<HDrilling>();

                foreach (double d in this.PointsPosition)
                {
                    HDrilling hdrill = new HDrilling(this.FaceNumber, this.EdgeBoreDiameter, this.EdgeBoreDepth, d, this.ZValue, Part);
                    TempHDrills.Add(hdrill);
                }

                foreach (StructXY xy in this.ListCamVBoreXY)
                {
                    VDrilling vdrill = new VDrilling(this.CamFaceNumber, xy.X, xy.Y, this.CamFaceBoreDiameter, this.CamFaceBoreDepth, Part);
                    Part.VDrillings.Add(vdrill);
                }

                foreach (PartFace f in pf.AssociatedPartFaces)
                {
                    if (!f.IsHorizontalFace)//如果关联的面是面5或面6
                    {
                        foreach (HDrilling hdrill in TempHDrills)
                        {
                            Point3d holeposition = hdrill.GetBorePosition();
                            holeposition = holeposition.TransformBy(Matrix3d.AlignCoordinateSystem(new Point3d(), Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, hdrill.Part.MPPoint, hdrill.Part.MPXAxis, hdrill.Part.MPYAxis, hdrill.Part.MPZAxis));
                            holeposition = Math.MathHelper.GetRotatedAndMovedPoint(holeposition, Part.TXRotation, Part.TYRotation, Part.TZRotation, Part.CenterVector);
                            holeposition = holeposition.TransformBy(Matrix3d.AlignCoordinateSystem(f.Part.MovedMPPoint, f.Part.MovedMPXAxis, f.Part.MovedMPYAxis, f.Part.MovedMPZAxis, new Point3d(), Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis));
                            double dimx = holeposition.X;
                            double dimy = holeposition.Y;

                            VDrilling vdrill = new VDrilling(f.FaceNumber, dimx, dimy, this.FaceBoreDiameter, this.FaceBoreDepth, f.Part);
                            f.Part.VDrillings.Add(vdrill);
                        }
                    }
                    else//TODO:如果关联的面是水平面 
                    {

                    }
                }

                if (this.EdgeBoreDiameter > 0 && this.EdgeBoreDepth > 0)
                {
                    Part.HDrillings.AddRange(TempHDrills);
                }
            }
        }

        private void camlockFaceChecker()
        {
            double value;
            if (double.TryParse(this.Par8, out value))
            {
                if (value == 5)
                {
                    this.CamFaceNumber = 5;
                    return;
                }
                else if (value == 6)
                {
                    this.CamFaceNumber = 6;
                    return;
                }
            }

            //this.IsValid = false;
            //this.writeError("Camlock指令参数par8错误：" + Par8, false);
        }

        private void depthChecker(string par6)
        {
            //string[] FDepths = par6.Split(',');
            //if (FDepths.Length >= 2)
            //{
            //    FaceBoreDepth = base.DoubleChecker(FDepths[0], @"Camlock/par6", true);
            //    CamFaceBoreDepth = base.DoubleChecker(FDepths[1], @"Camlock/par6", true);
            //}
            //else
            //{
            //    this.IsValid = false;
            //    this.writeError(string.Format("Camlock/par5机加工指令参数错误,分隔符数量不足:{0}", par6), false);
            //}
        }

        private void diameterChecker(string par5)
        {
            //string[] FDiams = par5.Split(',');
            //if (FDiams.Length >= 2)
            //{
            //    FaceBoreDiameter = base.DoubleChecker(FDiams[0], @"Camlock/par5", true);
            //    CamFaceBoreDiameter = base.DoubleChecker(FDiams[1], @"Camlock/par5", true);
            //}
            //else
            //{
            //    this.IsValid = false;
            //    this.writeError(string.Format("Camlock/par5机加工指令参数错误,分隔符数量不足:{0}", par5), false);
            //}
        }
    }
}
