using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Geometry;
using Dimeng.WoodEngine.Entities.Checks;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class PLINEMachiningToken : UnAssociativeToken
    {
        public PLINEMachiningToken(string token, string par1, string par2, string par3, string par4, string par5, string par6, string par7, string par8, string par9)
            : base(token, par1, par2, par3, par4, par5, par6, par7, par8, par9)
        {
            IsDrawOnly = false;
            this.OffsetZ = 0;
            this.OffsetX = 0;
            this.OffsetY = 0;
        }

        public override bool Valid(MachineTokenChecker check)
        {
            this.FaceNumber = check.FaceNumber(this.Token, 5, new int[] { 5, 6 }, this.Errors);
            this.Points = check.GetPoints(this.Par1, this.Errors);
            this.Bulges = check.GetBulges(this.Par2, this.Errors);

            getOffsetAndRotate(check);

            //对Bulges不足的情况，用0补上
            if (Bulges.Count < Points.Count)
            {
                int count = Points.Count - Bulges.Count;
                for (int i = 0; i < count; i++)
                {
                    Bulges.Add(0);
                }
            }

            this.ToolName = check.ToolName(this.Par7, "PLINE/刀具名称", this.Errors);
            this.ToolComp = check.GetToolComp(this.Par8, "PLINE/刀具补偿");

            if (this.Errors.Count == 0)
            {
                return true;
            }
            else return false;
        }

        private void getOffsetAndRotate(MachineTokenChecker check)
        {
            if (string.IsNullOrEmpty(this.Par5))
            {
                return;
            }

            string[] pars = this.Par5.Split('|');

            string[] offsets = pars[0].Split(';');
            if (offsets.Length == 3)
            {
                OffsetX = check.GetDoubleValue(offsets[0], "PLINE/OffsetX", false, this.Errors);
                OffsetY = check.GetDoubleValue(offsets[1], "PLine/OffsetY", false, this.Errors);
                OffsetZ = check.GetDoubleValue(offsets[2], "Pline/OffsetZ", false, this.Errors);
            }

            if (pars.Length > 1)
            {
                string[] rotates = pars[1].Split(';');
                if (rotates.Length == 3)
                {
                    RotateX = check.GetDoubleValue(rotates[0], "Pline/RotateX", false, this.Errors);
                    RotateY = check.GetDoubleValue(rotates[1], "Pline/RotateY", false, this.Errors);
                    RotateAngle = check.GetDoubleValue(rotates[2], "Pline/RotateAngle", false, this.Errors);
                }
            }
        }

        public List<Point3d> Points { get; private set; }
        public List<double> Bulges { get; private set; }
        public ToolComp ToolComp { get; private set; }
        public bool OnFace5 { get; private set; }
        public string ToolName { get; private set; }

        public double OffsetX { get; private set; }
        public double OffsetY { get; private set; }
        public double OffsetZ { get; private set; }
        public double RotateX { get; private set; }
        public double RotateY { get; private set; }
        public double RotateAngle { get; private set; }

        public override void ToMachining(double AssociatedDist, ToolFile toolFile)
        {
            //if (!toolFile.Tools.Any(it => it.ToolName == this.ToolName))
            //{
            //    this.writeError("未在刀具文件中找到铣刀：" + this.ToolName, false);
            //    return;
            //}
            pretreatPoints();

            Machinings.Routing route = new Machinings.Routing();
            route.ToolName = this.ToolName;
            route.ToolComp = this.ToolComp;
            route.Points = this.Points;
            route.Bulges = this.Bulges;
            //route.FeedSpeeds;
            route.OnFace5 = this.OnFace5;
            route.Part = this.Part;

            Part.Routings.Add(route);
        }

        private void pretreatPoints()
        {
            var list = new List<Point3d>();
            foreach (var pt in this.Points)
            {
                Point3d ptnew = new Point3d(pt.X + OffsetX, pt.Y + OffsetY, pt.Z + OffsetZ);
                list.Add(ptnew);
            }

            this.Points = list;
        }
    }
}
