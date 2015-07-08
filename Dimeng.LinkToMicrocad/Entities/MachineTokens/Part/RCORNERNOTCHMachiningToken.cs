using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Geometry;
using Dimeng.WoodEngine.Entities.Checks;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class RCORNERNOTCHMachiningToken : UnAssociativeToken
    {
        public RCORNERNOTCHMachiningToken(string token, string par1, string par2, string par3, string par4, string par5, string par6, string par7, string par8, string par9)
            : base(token, par1, par2, par3, par4, par5, par6, par7, par8, par9)
        {
        }

        public override bool Valid(MachineTokenChecker check)
        {
            this.FaceNumber = check.FaceNumber(this.Token, 12, new int[] { 5, 6 });
            this.EdgeNumber = check.EdgeNumber(this.Token, 14, new int[] { 1, 2, 3, 4, 5, 6, 7, 8 });

            if (FaceNumber == 6)
                OnFace5 = false;
            else OnFace5 = true;

            Radius = check.GetDoubleValue(Par1, "RCORNERNOTCH/圆半径", true, this.Errors);
            Depth = check.GetDoubleValue(Par3, "RCORNERNOTCH/深度", true, this.Errors);
            LeadIn = check.GetDoubleValue(Par4, "RCORNERNOTCH/进退刀距离", true, this.Errors);
            ToolName = check.ToolName(Par7, "RCORNERNOTCH/刀具名称");

            if (this.Errors.Count == 0)
            {
                return true;
            }
            else return false;
        }

        public double Radius { get; set; }
        public double Depth { get; set; }
        public double LeadIn { get; set; }
        public string ToolName { get; set; }
        public bool OnFace5 { get; set; }

        public override void ToMachining(double AssociatedDist, ToolFile toolFile)
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

            if (LeadIn > 0)
            {
                double rRadius = LeadIn;
                Point3d firstPt = new Point3d(Radius + rRadius, -rRadius, Depth);
                Point3d secondPt = new Point3d(Radius, 0, Depth);
                Point3d thirdPt = new Point3d(0, Radius, Depth);
                Point3d forthPt = new Point3d(-rRadius, Radius + rRadius, Depth);

                firstPt = firstPt.TransformBy(matrix1).TransformBy(matrix2);
                secondPt = secondPt.TransformBy(matrix1).TransformBy(matrix2);
                thirdPt = thirdPt.TransformBy(matrix1).TransformBy(matrix2);
                forthPt = forthPt.TransformBy(matrix1).TransformBy(matrix2);

                double firstBulge = -0.414214;
                ToolComp comp = ToolComp.None;

                if (firstPt.X >= thirdPt.X && firstPt.Y >= thirdPt.Y)
                {
                    comp = ToolComp.Left;
                    firstBulge *= -1;
                }
                else if (firstPt.X >= thirdPt.X && firstPt.Y < thirdPt.Y)
                {
                    comp = ToolComp.Right;
                }
                else if (firstPt.X < thirdPt.X && firstPt.Y < thirdPt.Y)
                {
                    comp = ToolComp.Left;
                    firstBulge *= -1;
                }
                else if (firstPt.X < thirdPt.X && firstPt.Y >= thirdPt.Y)
                {
                    comp = ToolComp.Right;
                }

                Machinings.Routing route = new Machinings.Routing();
                route.ToolName = ToolName;
                route.Points = new List<Point3d>() { firstPt, secondPt, thirdPt, forthPt };
                route.Part = Part;
                route.ToolComp = comp;
                route.Bulges = (new double[] { 0, firstBulge, 0, 0 }).ToList();
                if (FaceNumber == 5) route.OnFace5 = true;
                else route.OnFace5 = false;

                Part.Routings.Add(route);
            }
        }
    }
}
