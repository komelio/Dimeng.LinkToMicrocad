using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Geometry;
using Dimeng.WoodEngine.Entities.Checks;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class CHAMFERMachiningToken : UnAssociativeToken
    {
        public CHAMFERMachiningToken(string token, string par1, string par2, string par3, string par4, string par5, string par6, string par7, string par8, string par9)
            : base(token, par1, par2, par3, par4, par5, par6, par7, par8, par9)
        {

        }

        public override bool Valid(MachineTokenChecker check)
        {
            this.FaceNumber = check.FaceNumber(this.Token, 7, new int[] { 1, 2, 3, 4, 5, 6, 7, 8 });
            this.EdgeNumber = check.EdgeNumber(this.Token, 9, new int[] { 1, 2, 3, 4, 5, 6, 7, 8 });

            DistX = check.GetDoubleValue(Par1, "CHAMFER/X距离", true, this.Errors);
            DistY = check.GetDoubleValue(Par2, "CHAMFER/Y距离", true, this.Errors);
            Depth = check.GetDoubleValue(Par2, "CHAMFER/深度", true, this.Errors);

            this.LeadIn = check.GetDoubleValue(Par4, "CHAMFER/下刀引线长度", true, this.Errors);

            ToolName = check.ToolName(Par7, "CHAMFER/刀具名称");

            this.IsDrawOnly = check.GetBoolValue(Par8, "CHAMFER/只用于绘图", false, false, this.Errors);

            //TODO 纳入modelerror
            if (DistY <= 0 || DistX <= 0 || LeadIn < 0)
            {
                throw new Exception("距离值不能小于等于0");
            }

            if (this.Errors.Count == 0)
            {
                return true;
            }
            else return false;
        }

        public double DistX { get; private set; }
        public double DistY { get; private set; }
        public double Depth { get; private set; }
        public double LeadIn { get; private set; }
        public string ToolName { get; private set; }

        public override void ToMachining(double AssociatedDist, Entities.ToolFile toolFile)
        {
            //先以当前点为坐标系，建立两点坐标
            double xydist = System.Math.Sqrt(DistX * DistX + DistY * DistY);
            double cos = DistX / xydist;
            double sin = DistY / xydist;
            Point3d firstPt = new Point3d(DistX + LeadIn * cos, -LeadIn * sin, Depth);
            Point3d secondPt = new Point3d(-LeadIn * cos, DistY + LeadIn * sin, Depth);

            //把两点坐标，转换为世界坐标，再转为MP坐标系
            Point3d pt = Part.GetPartPointByNumber(EdgeNumber.ToString());
            Matrix3d matrix1 = Matrix3d.AlignCoordinateSystem(
                Point3d.Origin,
                Vector3d.XAxis,
                Vector3d.YAxis,
                Vector3d.ZAxis,
                pt,
                (Part.MachinePoint.IsRotated) ? this.GetPointYAxis(EdgeNumber) : this.GetPointXAxis(EdgeNumber),
                (Part.MachinePoint.IsRotated) ? this.GetPointXAxis(EdgeNumber) : this.GetPointYAxis(EdgeNumber),
                Vector3d.ZAxis);
            Matrix3d matrix2 = Matrix3d.AlignCoordinateSystem(
                Part.MPPoint,
                Part.MPXAxis,
                Part.MPYAxis,
                Part.MPZAxis,
                Point3d.Origin,
                Vector3d.XAxis,
                Vector3d.YAxis,
                Vector3d.ZAxis);

            firstPt = firstPt.TransformBy(matrix1).TransformBy(matrix2);
            secondPt = secondPt.TransformBy(matrix1).TransformBy(matrix2);

            //todo:转换后z值变成负数了，转会正数，有空再研究原因
            firstPt = new Point3d(firstPt.X, firstPt.Y, Depth);
            secondPt = new Point3d(secondPt.X, secondPt.Y, Depth);

            ToolComp comp = ToolComp.None;

            if (firstPt.X >= secondPt.X && firstPt.Y >= secondPt.Y)
                comp = ToolComp.Left;
            else if (firstPt.X >= secondPt.X && firstPt.Y < secondPt.Y)
                comp = ToolComp.Right;
            else if (firstPt.X < secondPt.X && firstPt.Y < secondPt.Y)
                comp = ToolComp.Left;
            else if (firstPt.X < secondPt.X && firstPt.Y >= secondPt.Y)
                comp = ToolComp.Right;

            List<Point3d> points = new List<Point3d>() { firstPt, secondPt };

            Machinings.Routing route = new Machinings.Routing();
            route.ToolName = ToolName;
            route.Points = points;
            route.Part = Part;
            route.ToolComp = comp;
            route.Bulges = (new double[] { 0, 0 }).ToList();
            if (FaceNumber == 5) route.OnFace5 = true;
            else route.OnFace5 = false;

            Part.Routings.Add(route);
        }
    }
}
