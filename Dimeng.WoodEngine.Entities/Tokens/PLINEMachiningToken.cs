using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Geometry;
//using Dimeng.WoodEngine.Data.Job.Product.Machinings;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class PLINEMachiningToken : BaseToken
    {
        public PLINEMachiningToken(string token, string par1, string par2, string par3, string par4, string par5, string par6, string par7, string par8, string par9, int row, int column, Part p)
            : base(token, par1, par2, par3, par4, par5, par6, par7, par8, par9, row, column, p)
        {
            IsDrawOnly = false;
        }

        public override bool Valid(Logger logger)
        {
            this.logger = logger;

            base.faceNumberChecker(this.Token, 5, new int[] { 5, 6 });

            this.checkPoints();

            Bulges = base.pointsChecker(this.Par2);

            //对Bulges不足的情况，用0补上
            if (Bulges.Count < Points.Count)
            {
                for (int i = 0; i < Points.Count - Bulges.Count; i++)
                {
                    Bulges.Add(0);
                }
            }

            this.ToolName = base.notEmptyStringChecker(this.Par7, "PLINE/刀具名称");

            ToolComp = base.toolCompChecker(this.Par8, "PLINE/刀具补偿");

            if (this.FaceNumber == 5)
            {
                OnFace5 = true;
            }
            else if (this.FaceNumber == 6)
            {
                OnFace5 = false;
            }

            return this.IsValid;
        }

        public List<Point3d> Points { get; private set; }
        public List<double> Bulges { get; private set; }
        public ToolComp ToolComp { get; private set; }
        public bool OnFace5 { get; private set; }
        public string ToolName { get; private set; }

        private void checkPoints()
        {
            Points = Par1.Split('|')
                         .Select(s => FromStringToPoint3d(s.Split(';')))
                         .ToList();
        }
        private Point3d FromStringToPoint3d(string[] p)
        {
            if (p.Length != 3)
            {
                this.IsValid = false;
                this.writeError("非法的点数据:" + this.Par1, false);
                return Point3d.Origin;
            }

            double x = base.DoubleChecker(p[0], "PLINE/点坐标", false);
            double y = base.DoubleChecker(p[1], "PLINE/点坐标", false);
            double z = base.DoubleChecker(p[2], "PLINE/点坐标", false);

            return new Point3d(x, y, z);
        }

        public override void ToMachining(double AssociatedDist, ToolFile toolFile)
        {
            //if (!toolFile.Tools.Any(it => it.ToolName == this.ToolName))
            //{
            //    this.writeError("未在刀具文件中找到铣刀：" + this.ToolName, false);
            //    return;
            //}

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
    }
}
