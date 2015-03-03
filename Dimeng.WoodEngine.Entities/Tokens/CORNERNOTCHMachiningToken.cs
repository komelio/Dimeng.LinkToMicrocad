using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Geometry;
using Dimeng.WoodEngine.Entities.Checks;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class CORNERNOTCHMachiningToken : BaseToken
    {
        public CORNERNOTCHMachiningToken(string token, string par1, string par2, string par3, string par4, string par5, string par6, string par7, string par8, string par9)
            : base(token, par1, par2, par3, par4, par5, par6, par7, par8, par9)
        {

        }

        public override bool Valid(MachineTokenChecker check)
        {
            this.FaceNumber = check.FaceNumber(this.Token, 11, new int[] { 5, 6 });
            this.EdgeNumber = check.EdgeNumber(this.Token, 13, new int[] { 1, 2, 3, 4, 5, 6, 7, 8 });

            XDist = check.GetDoubleValue(Par1, "直角切口/X上距离", false, check.Errors);
            YDist = check.GetDoubleValue(Par2, "直角切口/Y上距离", false, check.Errors);
            Depth = check.GetDoubleValue(Par3, "直角切口/深度", true, check.Errors);
            LeadIn = string.IsNullOrEmpty(Par4) ? 0 : check.GetDoubleValue(Par4, "直角切口/进退刀距离", true, check.Errors);
            ToolName = check.ToolName(this.Par7, "直角切口/刀具名称");
            IsDrawOnly = check.GetBoolValue(this.Par8, "直接切口/仅用于绘图", false, false, check.Errors);

            if (check.Errors.Count == 0)
            {
                return true;
            }
            else return false;
        }

        public double XDist { get; private set; }
        public double YDist { get; private set; }
        public double Depth { get; private set; }
        public double LeadIn { get; private set; }
        public string ToolName { get; private set; }

        public override void ToMachining(double AssociatedDist, Entities.ToolFile toolFile)
        {
            //由于这个指令是指定了板件的1-8个点进行定位的，所以要把空间的绝对坐标换算回机加工原点的相对坐标 
            //matrix1负责把所在点的坐标转换为板件中心的坐标
            //matrix2负责把板件中心的坐标，再转换为MP中心的坐标
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

            //板件如果旋转，则生成的坐标点是不一样的
            List<Point3d> Points = new List<Point3d>();
            Point3d firstPt = new Point3d(XDist, -LeadIn, Depth);
            Point3d secondPt = new Point3d(XDist, YDist, Depth);
            Point3d thirdPt = new Point3d(-LeadIn, YDist, Depth);
            firstPt = firstPt.TransformBy(matrix1).TransformBy(matrix2);
            secondPt = secondPt.TransformBy(matrix1).TransformBy(matrix2);
            thirdPt = thirdPt.TransformBy(matrix1).TransformBy(matrix2);
            Points.Add(new Point3d(firstPt.X, firstPt.Y, Depth));
            Points.Add(new Point3d(secondPt.X, secondPt.Y, Depth));
            Points.Add(new Point3d(thirdPt.X, thirdPt.Y, Depth));

            //判断刀补的方向
            ToolComp comp = ToolComp.None;

            if (firstPt.X >= thirdPt.X && firstPt.Y >= thirdPt.Y)
                comp = ToolComp.Left;
            else if (firstPt.X >= thirdPt.X && firstPt.Y < thirdPt.Y)
                comp = ToolComp.Right;
            else if (firstPt.X < thirdPt.X && firstPt.Y < thirdPt.Y)
                comp = ToolComp.Left;
            else if (firstPt.X < thirdPt.X && firstPt.Y >= thirdPt.Y)
                comp = ToolComp.Right;

            Machinings.Routing route = new Machinings.Routing();
            route.ToolName = ToolName;
            route.Points = Points;
            route.Part = Part;
            route.ToolComp = comp;
            route.Bulges = (new double[] { 0, 0, 0 }).ToList();
            if (FaceNumber == 5) route.OnFace5 = true;
            else route.OnFace5 = false;

            Part.Routings.Add(route);
        }


    }
}
