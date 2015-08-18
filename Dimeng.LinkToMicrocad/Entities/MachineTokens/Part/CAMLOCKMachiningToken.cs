using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Geometry;
using Dimeng.WoodEngine.Entities.Machinings;
using Dimeng.WoodEngine.Entities.Checks;
using Dimeng.LinkToMicrocad.Logging;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class CAMLOCKMachiningToken : AssociativeToken
    {
        public CAMLOCKMachiningToken(string token, string par1, string par2, string par3,
            string par4, string par5, string par6, string par7, string par8, string par9)
            : base(token, par1, par2, par3, par4, par5, par6, par7, par8, par9)
        {
            IsDrawOnly = false;
        }

        public override bool Valid(MachineTokenChecker check)
        {
            //todo:其实深度和直径是用逗号区分的,存在两种尺寸并排的情况要处理
            this.FaceNumber = check.FaceNumber(this.Token, 7, new int[] { 1, 2, 3, 4 }, this.Errors);
            PointsPosition = check.PointPositions(this.Par1, this.Errors);
            EdgeBoreDiameter = check.GetDoubleValue(this.Par2, @"Camlock/边孔直径", true, this.Errors);
            EdgeBoreDepth = check.GetDoubleValue(this.Par3, @"Camlock/边孔深度", true, this.Errors);
            ZValue = (string.IsNullOrEmpty(this.Par4)) ?
                Part.Thickness / 2 : check.GetDoubleValue(this.Par4, @"Camlock/par4", true, this.Errors);
            diameterChecker(this.Par5, check);
            depthChecker(this.Par6, check);
            Backset = check.GetDoubleValue(this.Par7, @"Camlock/par7", false, this.Errors);

            camlockFaceChecker();

            this.DrillFromOppsiteFace = check.GetBoolValue(this.Par9, @"Camlock/par9", false, false, this.Errors);
            ListCamVBoreXY = GetCamVBoreXYList();

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

        public override void ToMachining(double tolerenceDist, Entities.ToolFile toolFile)
        {
            Logger.GetLogger().Debug(string.Format("Camlock ToMachining!{0}/{1}/{2}/{3}/{4}/{5}", this.Token, this.Par1, this.Par2, this.Par3, this.Par4, this.Part));
            FindAssociatedFaces(0, tolerenceDist);

            PartFace pf = this.Part.GetPartFaceByNumber(FaceNumber);
            if (this.AssociatedPartFaces.Count != 0)//数量不为0，说明有关联的板件
            {
                Logger.GetLogger().Debug("Camlock Faces:" + this.AssociatedPartFaces.Count.ToString());

                List<HDrilling> TempHDrills = new List<HDrilling>();

                foreach (double d in this.PointsPosition)
                {
                    HDrilling hdrill = new HDrilling(this.FaceNumber, this.EdgeBoreDiameter, this.EdgeBoreDepth, d, (this.CamFaceNumber==5?this.ZValue:this.Part.Thickness-this.ZValue), Part, this);
                    TempHDrills.Add(hdrill);
                }

                foreach (StructXY xy in this.ListCamVBoreXY)
                {
                    VDrilling vdrill = new VDrilling(this.CamFaceNumber, xy.X, xy.Y, this.CamFaceBoreDiameter, this.CamFaceBoreDepth, Part, this);
                    Part.VDrillings.Add(vdrill);
                }

                foreach (PartFace f in this.AssociatedPartFaces)
                {
                    Logger.GetLogger().Debug(string.Format("CamlockAssociateFace:{0}/{1}", f.Part.ToString(), f.FaceNumber));
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

                            VDrilling vdrill = new VDrilling(f.FaceNumber, dimx, dimy, this.FaceBoreDiameter, this.FaceBoreDepth, f.Part, this);
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
            if (string.IsNullOrEmpty(this.Par8))
            {
                Logger.GetLogger().Debug("Camlock Par8为空");
                this.CamFaceNumber = 5;
                return;
            }

            double value;
            if (double.TryParse(this.Par8, out value))
            {
                if (value == 5)
                {
                    Logger.GetLogger().Debug("Camlock Par8为5");
                    this.CamFaceNumber = 5;
                    return;
                }
                else if (value == 6)
                {
                    Logger.GetLogger().Debug("Camlock Par8为6");
                    this.CamFaceNumber = 6;
                    return;
                }
            }
            else
            {
                Logger.GetLogger().Debug("Camlock Par8无法识别" + this.Par8);
                this.CamFaceNumber = 5;
                return;
            }
        }

        private void depthChecker(string par6, MachineTokenChecker checker)
        {
            string[] FDepths = par6.Split(',');
            if (FDepths.Length >= 2)
            {
                FaceBoreDepth = checker.GetDoubleValue(FDepths[0], @"Camlock/par6", true, this.Errors);
                CamFaceBoreDepth = checker.GetDoubleValue(FDepths[1], @"Camlock/par6", true, this.Errors);
            }
            else
            {
                this.Errors.Add(new ModelError("Wrong camlock depth:" + par6));
            }
        }

        private void diameterChecker(string par5, MachineTokenChecker checker)
        {
            string[] FDiams = par5.Split(',');
            if (FDiams.Length >= 2)
            {
                FaceBoreDiameter = checker.GetDoubleValue(FDiams[0], @"Camlock/par5", true, this.Errors);
                CamFaceBoreDiameter = checker.GetDoubleValue(FDiams[1], @"Camlock/par5", true, this.Errors);
            }
            else
            {
                this.Errors.Add(new ModelError("Wrong camlock diameter:" + par5));
            }
        }
    }
}
