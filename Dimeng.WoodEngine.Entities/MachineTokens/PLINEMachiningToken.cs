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
        }

        public override bool Valid(MachineTokenChecker check)
        {

            this.FaceNumber = check.FaceNumber(this.Token, 5, new int[] { 5, 6 });
            this.Points = check.GetPoints(this.Par1);
            this.Bulges = check.GetBulges(this.Par2);

            //对Bulges不足的情况，用0补上
            if (Bulges.Count < Points.Count)
            {
                for (int i = 0; i < Points.Count - Bulges.Count; i++)
                {
                    Bulges.Add(0);
                }
            }

            this.ToolName = check.ToolName(this.Par7, "PLINE/刀具名称");
            this.ToolComp = check.GetToolComp(this.Par8, "PLINE/刀具补偿");

            if (check.Errors.Count == 0)
            {
                return true;
            }
            else return false;
        }

        public List<Point3d> Points { get; private set; }
        public List<double> Bulges { get; private set; }
        public ToolComp ToolComp { get; private set; }
        public bool OnFace5 { get; private set; }
        public string ToolName { get; private set; }

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
