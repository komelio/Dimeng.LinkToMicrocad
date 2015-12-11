using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Geometry;
using Dimeng.WoodEngine.Entities.Checks;
using Dimeng.WoodEngine.Entities.Machinings;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class ROUTEDHOLEMachiningToken : BaseToken
    {
        public ROUTEDHOLEMachiningToken(string token, string par1, string par2, string par3, string par4, string par5, string par6, string par7, string par8, string par9)
            : base(token, par1, par2, par3, par4, par5, par6, par7, par8, par9)
        {

        }

        public override bool Valid(MachineTokenChecker check)
        {
            this.FaceNumber = check.FaceNumber(Token, 10, new int[] { 5, 6 }, this.Errors);
            if (FaceNumber == 6)
                this.OnFace5 = false;
            else this.OnFace5 = true;

            StartX = check.GetDoubleValue(Par1, "铣圆指令/X起始坐标", false, this.Errors);
            StartY = check.GetDoubleValue(Par2, "铣圆指令/Y起始坐标", false, this.Errors);
            Depth = check.GetDoubleValue(Par3, "铣圆指令/深度", true, this.Errors);
            Radius = check.GetDoubleValue(Par4, "铣圆指令/圆半径", true, this.Errors);
            IsPocket = check.GetBoolValue(Par5, "铣圆指令/袋式加工", false, false, this.Errors);
            ToolName = check.ToolName(Par7, "铣圆指令/刀具名称", this.Errors);
            IsDrawOnly = check.GetBoolValue(Par8, "铣圆指令/仅用于绘图", false, false, this.Errors);

            if (this.Errors.Count == 0)
            {
                return true;
            }
            else return false;
        }

        public double StartX { get; set; }
        public double StartY { get; set; }
        public double Depth { get; set; }
        public double Radius { get; set; }
        public bool IsPocket { get; set; }
        public string ToolName { get; set; }
        public bool OnFace5 { get; set; }

        public override void ToMachining(double AssociatedDist, ToolFile toolFile)
        {
            double diameter = toolFile.GetRouteToolByName(this.ToolName).Diameter;
            if (diameter >= Radius)
                throw new Exception("不支持加工圆半径小于铣刀半径的情况");

            double smallRadius = (Radius < 12.7) ? Radius : 12.7;//TODO:MV里这是一个定制，如果大于大员的半径，就用大圆的半径

            List<Point3d> points = new List<Point3d>();
            List<double> bulges = new List<double>();

            //points.Add(new Point3d(StartX - Radius + smallRadius, StartY + smallRadius, 0));//下刀深度为0
            points.Add(new Point3d(StartX - Radius, StartY, Depth));
            points.Add(new Point3d(StartX, StartY - Radius, Depth));
            points.Add(new Point3d(StartX + Radius, StartY, Depth));
            points.Add(new Point3d(StartX, StartY + Radius, Depth));
            points.Add(new Point3d(StartX - Radius, StartY, Depth));
            //points.Add(new Point3d(StartX - Radius + smallRadius, StartY - smallRadius, 0));
            //bulges.AddRange(new double[] { -0.414214, -0.414214, -0.414214, -0.414214, -0.414214, -0.414214, -0.414214 });
            bulges.AddRange(new double[] { -0.414214, -0.414214, -0.414214, -0.414214, -0.414214 });
            Machinings.Routing route = new Machinings.Routing();
            route.Bulges = bulges;
            route.Points = points;
            route.ToolComp = ToolComp.Right;
            route.Part = this.Part;
            route.OnFace5 = this.OnFace5;
            route.ToolName = this.ToolName;

            Part.Routings.Add(route);

            if (IsPocket || this.Depth >= Part.Thickness)
            {
                var pocket = new Pocket();
                pocket.Points.Add(new Point3d(StartX - Radius + diameter / 2, StartY, Depth));
                pocket.Points.Add(new Point3d(StartX, StartY - Radius + diameter / 2, Depth));
                pocket.Points.Add(new Point3d(StartX + Radius - diameter / 2, StartY, Depth));
                pocket.Points.Add(new Point3d(StartX, StartY + Radius - diameter / 2, Depth));
                pocket.Bulges.AddRange(new double[] { -0.414214, -0.414214, -0.414214, -0.414214 });
                pocket.FaceNumber = this.FaceNumber;
                pocket.Part = this.Part;
                pocket.Depth = this.Depth;

                Part.Pockets.Add(pocket);

                //TODO:这个未完成
                PocketMachining(diameter);
            }
        }

        private void PocketMachining(double diameter)
        {
            //List<Point3d> points = new List<Point3d>();
            //List<double> bulges = new List<double>();

            //throw new NotImplementedException();
        }
    }
}
