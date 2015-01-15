using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Geometry;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class CUTOUTMachiningToken : BaseToken
    {
        public CUTOUTMachiningToken(string token, string par1, string par2, string par3, string par4, string par5, string par6, string par7, string par8, string par9, int row, int column, Part p)
            : base(token, par1, par2, par3, par4, par5, par6, par7, par8, par9, row, column, p)
        {
        }

        public override bool Valid(Logger logger)
        {
            this.logger = logger;

            base.faceNumberChecker(this.Token, 6, new int[] { 5, 6 });
            StartX = base.DoubleChecker(Par1, "CUTOUT/X起始坐标", false);
            StartY = base.DoubleChecker(Par2, "CUTOUT/Y起始坐标", false);
            Depth = base.DoubleChecker(Par3, "CUTOUT/深度", true);
            EndX = base.DoubleChecker(Par4, "CUTOUT/X结束坐标", false);
            EndY = base.DoubleChecker(Par5, "CUTOUT/Y起始坐标", false);
            IsPocket = base.BoolChecker(Par6, "CUTOUT/袋式加工", false, false);
            ToolName = base.notEmptyStringChecker(Par7, "CUTOUT/刀具名称");
            IsDrawOnly = base.BoolChecker(Par8, "CUTOUT/仅用于图形绘制", false, false);

            lowX = (StartX >= EndX) ? EndX : StartX;
            lowY = (StartY >= EndY) ? EndY : StartY;
            highX = (StartX >= EndX) ? StartX : EndX;
            highY = (StartY >= EndY) ? StartY : EndY;

            return this.IsValid;
        }

        public double StartX { get; set; }
        public double StartY { get; set; }
        public double Depth { get; set; }
        public double EndX { get; set; }
        public double EndY { get; set; }
        public bool IsPocket { get; set; }
        public string ToolName { get; set; }
        public bool OnFace5 { get; set; }

        private double lowX;
        private double lowY;
        private double highX;
        private double highY;

        public override void ToMachining(double AssociatedDist, Entities.ToolFile toolFile)
        {
            List<Point3d> points = new List<Point3d>()
                {
                    new Point3d(lowX/2+highX/2,lowY/2+highY/2,Depth),
                    new Point3d(lowX,lowY/2+highY/2,Depth),
                    new Point3d(lowX,highY,Depth),
                    new Point3d(highX,highY,Depth),
                    new Point3d(highX,lowY,Depth),
                    new Point3d(lowX,lowY,Depth),
                    new Point3d(lowX,lowY+highY/4-lowY/4,Depth),
                };
            List<double> bulges = new List<double>() { 0, 0, 0, 0, 0, 0, 0 };
            Machinings.Routing route = new Machinings.Routing();
            route.Bulges = bulges;
            route.Points = points;
            route.ToolComp = ToolComp.Left;
            route.Part = this.Part;
            route.OnFace5 = this.OnFace5;
            route.ToolName = this.ToolName;

            Part.Routings.Add(route);

            //TODO:不知道刀的大小，如何生成Pocket的路径？
            //可能需要在分析时再来生成路径，晕死
            if (IsPocket)
            {
                PocketMachining(toolFile);
            }
        }

        private void PocketMachining(Entities.ToolFile toolFile)
        {
            List<Point3d> points = new List<Point3d>();
            List<double> bulges = new List<double>();

            double diameter = toolFile.GetRouteToolByName(this.ToolName).Diameter;
            int row = (int)System.Math.Ceiling(System.Math.Abs(StartX - EndX) / diameter) - 1;

            double mod = System.Math.Abs(StartX - EndX) % diameter;

            for (int i = 0; i < row; i++)
            {
                if (i != row - 1)
                {
                    AddLineToPocket(points, bulges, diameter, i);
                }
                else
                {
                    if (mod == 0)//如果是整数行
                        AddLineToPocket(points, bulges, diameter, i);
                    else
                    {
                        if (i % 2 == 0)//整除2，表明为基数列
                        {
                            points.Add(new Point3d(lowX + diameter * (i - 1) + diameter * mod / diameter, lowY + diameter, Depth));
                            points.Add(new Point3d(lowX + diameter * (i - 1) + diameter * mod / diameter, highY - diameter, Depth));
                            bulges.Add(0);
                            bulges.Add(0);
                        }
                        else
                        {
                            points.Add(new Point3d(lowX + diameter * (i - 1) + diameter * mod / diameter, highY - diameter, Depth));
                            points.Add(new Point3d(lowX + diameter * (i - 1) + diameter * mod / diameter, lowY + diameter, Depth));
                            bulges.Add(0);
                            bulges.Add(0);
                        }
                    }
                }
            }

            Machinings.Routing route = new Machinings.Routing();
            route.Bulges = bulges;
            route.Points = points;
            route.ToolComp = ToolComp.Left;
            route.Part = this.Part;
            route.OnFace5 = this.OnFace5;
            route.ToolName = this.ToolName;

            Part.Routings.Add(route);
        }

        private void AddLineToPocket(List<Point3d> points, List<double> bulges, double diameter, int i)
        {
            if (i % 2 == 0)//整除2，表明为基数列
            {
                points.Add(new Point3d(lowX + diameter * i, lowY + diameter, Depth));
                points.Add(new Point3d(lowX + diameter * i, highY - diameter, Depth));
                bulges.Add(0);
                bulges.Add(0);
            }
            else
            {
                points.Add(new Point3d(lowX + diameter * i, highY - diameter, Depth));
                points.Add(new Point3d(lowX + diameter * i, lowY + diameter, Depth));
                bulges.Add(0);
                bulges.Add(0);
            }
        }
    }
}
