using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Geometry;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class CHAMFERMachiningToken : BaseToken
    {
        public CHAMFERMachiningToken(string token, string par1, string par2, string par3, string par4, string par5, string par6, string par7, string par8, string par9, int row, int column, Part p)
            : base(token, par1, par2, par3, par4, par5, par6, par7, par8, par9, row, column, p)
        {

        }

        public override bool Valid(Logger logger)
        {
            this.logger = logger;

            base.faceNumberChecker(this.Token, 7, new int[] { 1, 2, 3, 4, 5, 6, 7, 8 });
            base.edgeNumberChecker(this.Token, 9, new int[] { 1, 2, 3, 4, 5, 6, 7, 8 });

            DistX = base.DoubleChecker(Par1, "CHAMFER/X距离", true);
            DistY = base.DoubleChecker(Par2, "CHAMFER/Y距离", true);
            Depth = base.DoubleChecker(Par2, "CHAMFER/深度", true);

            this.LeadIn = base.DoubleChecker(Par4, "CHAMFER/下刀引线长度", true);

            ToolName = base.notEmptyStringChecker(Par7, "CHAMFER/刀具名称");

            this.IsDrawOnly = base.BoolChecker(Par8, "CHAMFER/只用于绘图", false, false);

            if (DistY <= 0 || DistX <= 0 || LeadIn < 0)
            {
                throw new Exception("距离值不能小于等于0");
            }

            return this.IsValid;
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
