﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Geometry;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class _3SIDEDNOTCHMachiningToken : BaseToken
    {
        public _3SIDEDNOTCHMachiningToken(string token, string par1, string par2, string par3, string par4, string par5, string par6, string par7, string par8, string par9, int row, int column, Part p)
            : base(token, par1, par2, par3, par4, par5, par6, par7, par8, par9, row, column, p)
        {
        }

        public override bool Valid(Logger logger)
        {
            this.logger = logger;

            base.edgeNumberChecker(Token, 13, new int[] { 1, 2, 3, 4 });
            base.faceNumberChecker(Token, 11, new int[] { 5, 6 });

            StartX = base.DoubleChecker(Par1, "三面切口指令/X起始坐标", false);
            StartY = base.DoubleChecker(Par2, "三面切口指令/Y起始坐标", false);
            Depth = base.DoubleChecker(Par3, "三面切口指令/深度", true);

            if (EdgeNumber == 1 || EdgeNumber == 2)
            {
                EndY = StartY;
            }
            else
            {
                EndY = base.DoubleChecker(Par5, "三面切口指令/Y起始坐标", false);
            }

            if (EdgeNumber == 3 || EdgeNumber == 4)
            {
                EndX = StartX;
            }
            else
            {
                EndX = base.DoubleChecker(Par4, "三面切口指令/Y起始坐标", false);
            }

            this.LeadIn = base.DoubleChecker(Par6, "三面切口指令/下刀引线长度", false);

            ToolName = base.notEmptyStringChecker(Par7, "三面切口指令/刀具名称");

            IsDrawOnly = base.BoolChecker(Par8, "三面切口指令/只用于绘图", false, false);

            return this.IsValid;
        }

        public double StartX { get; set; }
        public double StartY { get; set; }
        public double Depth { get; set; }
        public double EndX { get; set; }
        public double EndY { get; set; }
        public double LeadIn { get; set; }
        public string ToolName { get; set; }

        public override void ToMachining(double AssociatedDist, Entities.ToolFile toolFile)
        {
            //指令根据板件的1-4个边进行定位
            //需要根据面的方位来生成加工的位置
            //幸运的是，不需要经过太多的坐标转换

            List<Point3d> Points = new List<Point3d>();
            ToolComp comp = ToolComp.None;

            if (EdgeNumber == 1)
            {
                Point3d firstPt = new Point3d(StartX, -LeadIn, Depth);
                Point3d secondPt = new Point3d(StartX, StartY, Depth);
                Point3d thirdPt = new Point3d(EndX, EndY, Depth);
                Point3d forthPt = new Point3d(EndX, -LeadIn, Depth);
                Points.Add(firstPt);
                Points.Add(secondPt);
                Points.Add(thirdPt);
                Points.Add(forthPt);

                comp = ToolComp.Left;

                Machinings.Routing route = new Machinings.Routing();
                route.ToolName = ToolName;
                route.Points = Points;
                route.Part = Part;
                route.ToolComp = comp;
                route.Bulges = (new double[] { 0, 0, 0, 0 }).ToList();
                if (FaceNumber == 5) route.OnFace5 = true;
                else route.OnFace5 = false;

                Part.Routings.Add(route);
            }
            else if (EdgeNumber == 2)
            {
                //double pl = (Part.MachinePoint.IsRotated) ? Part.Width : Part.Length;
                double pw = (!Part.MachinePoint.IsRotated) ? Part.Width : Part.Length;
                Point3d firstPt = new Point3d(StartX, pw + LeadIn, Depth);
                Point3d secondPt = new Point3d(StartX, pw - StartY, Depth);
                Point3d thirdPt = new Point3d(EndX, pw - EndY, Depth);
                Point3d forthPt = new Point3d(EndX, pw + LeadIn, Depth);
                Points.Add(firstPt);
                Points.Add(secondPt);
                Points.Add(thirdPt);
                Points.Add(forthPt);

                comp = ToolComp.Right;

                Machinings.Routing route = new Machinings.Routing();
                route.ToolName = ToolName;
                route.Points = Points;
                route.Part = Part;
                route.ToolComp = comp;
                route.Bulges = (new double[] { 0, 0, 0, 0 }).ToList();
                if (FaceNumber == 5) route.OnFace5 = true;
                else route.OnFace5 = false;

                Part.Routings.Add(route);
            }
            else if (EdgeNumber == 3)
            {
                //double pl = (Part.MachinePoint.IsRotated) ? Part.Width : Part.Length;
                //double pw = (!Part.MachinePoint.IsRotated) ? Part.Width : Part.Length;
                Point3d firstPt = new Point3d(-LeadIn, StartY, Depth);
                Point3d secondPt = new Point3d(StartX, StartY, Depth);
                Point3d thirdPt = new Point3d(EndX, EndY, Depth);
                Point3d forthPt = new Point3d(-LeadIn, EndY, Depth);
                Points.Add(firstPt);
                Points.Add(secondPt);
                Points.Add(thirdPt);
                Points.Add(forthPt);

                comp = ToolComp.Right;

                Machinings.Routing route = new Machinings.Routing();
                route.ToolName = ToolName;
                route.Points = Points;
                route.Part = Part;
                route.ToolComp = comp;
                route.Bulges = (new double[] { 0, 0, 0, 0 }).ToList();
                if (FaceNumber == 5) route.OnFace5 = true;
                else route.OnFace5 = false;

                Part.Routings.Add(route);
            }
            else if (EdgeNumber == 4)
            {
                double pl = (Part.MachinePoint.IsRotated) ? Part.Width : Part.Length;
                //double pw = (!Part.MachinePoint.IsRotated) ? Part.Width : Part.Length;
                Point3d firstPt = new Point3d(pl + LeadIn, StartY, Depth);
                Point3d secondPt = new Point3d(pl - StartX, StartY, Depth);
                Point3d thirdPt = new Point3d(pl - EndX, EndY, Depth);
                Point3d forthPt = new Point3d(pl + LeadIn, EndY, Depth);
                Points.Add(firstPt);
                Points.Add(secondPt);
                Points.Add(thirdPt);
                Points.Add(forthPt);

                comp = ToolComp.Left;

                Machinings.Routing route = new Machinings.Routing();
                route.ToolName = ToolName;
                route.Points = Points;
                route.Part = Part;
                route.ToolComp = comp;
                route.Bulges = (new double[] { 0, 0, 0, 0 }).ToList();
                if (FaceNumber == 5) route.OnFace5 = true;
                else route.OnFace5 = false;

                Part.Routings.Add(route);
            }
        }
    }
}
