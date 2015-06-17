using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Geometry;
using Dimeng.WoodEngine.Entities.Checks;
using Dimeng.WoodEngine.Entities.Machinings;
using Autodesk.AutoCAD.DatabaseServices;
//using Dimeng.WoodEngine.Data.Job.Product.Machinings;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class DADOMachiningToken : AssociativeToken
    {
        public DADOMachiningToken(string token, string par1, string par2, string par3, string par4, string par5, string par6, string par7, string par8, string par9)
            : base(token, par1, par2, par3, par4, par5, par6, par7, par8, par9)
        {
            IsDrawOnly = false;
        }

        public override bool Valid(MachineTokenChecker check)
        {

            this.FaceNumber = check.FaceNumber(this.Token, 4, new int[] { 1, 2, 3, 4 });
            LeadIn = (string.IsNullOrEmpty(Par1)) ? 0 : check.GetDoubleValue(Par1, "DADO/进刀引线长度", true, check.Errors);
            ReverseDirection = check.GetBoolValue(Par2, "DADO/反向加工", false, false, check.Errors);
            Depth = check.GetDoubleValue(Par3, "DADO/加工深度", true, check.Errors);
            LeadOut = (string.IsNullOrEmpty(Par4)) ? 0 : check.GetDoubleValue(Par4, "DADO/退刀引线长度", true, check.Errors);
            DADOThick = check.GetDoubleValue(Par5, "DADO/槽宽", true, check.Errors);
            ToolName = check.ToolName(Par7, "DADO/刀具名称");
            Penetration = check.GetDoubleValue(Par8, "DADO/背板与槽间隙", false, check.Errors);

            if (check.Errors.Count == 0)
            {
                return true;
            }
            else return false;
        }

        public double LeadIn { get; private set; }//进刀距离
        public bool ReverseDirection { get; private set; }//反向加工
        public double Depth { get; private set; }//开槽深度
        public double LeadOut { get; private set; }//出刀距离
        public double DADOThick { get; private set; }//槽宽
        public double LockJoint { get; private set; }//锁结构
        public string ToolName { get; private set; }//刀具名称
        public double Penetration { get; private set; } //嵌入深度，决定是否关联的距离
        public string TongueToolName1 { get; private set; }
        public string TongueToolName2 { get; private set; }

        //TODO:关联的是一个面，还是多个面？
        //似乎没有必要同时关联多个面
        public List<PartFace> AssociatedPartFaceList = new List<PartFace>();

        public override void ToMachining(double tolerenceDist, ToolFile toolFile)
        {
            var tool = toolFile.Tools.Find(it => it.ToolType == ToolType.Router && it.ToolName.ToUpper() == this.ToolName.ToUpper());
            if (tool == null)
            {
                throw new Exception("Unknow tool in DADO token : " + this.ToolName);
            }


            //得到当前面的关联面
            base.FindAssociatedFaces(this.Penetration, tolerenceDist);

            //当前的face
            var currentFace = this.Part.GetPartFaceByNumber(this.FaceNumber);

            foreach (var f in this.AssociatedPartFaces)
            {
                //得到一个转换矩阵，将
                var matrix = Matrix3d.AlignCoordinateSystem(f.Part.MovedMPPoint,
                                                            f.Part.MovedMPXAxis,
                                                            f.Part.MovedMPYAxis,
                                                            f.Part.MovedMPZAxis,
                                                            Point3d.Origin,
                                                            Vector3d.XAxis,
                                                            Vector3d.YAxis,
                                                            Vector3d.ZAxis);

                //step1: 获得板件的机加工原点
                //step2：将原点映射到板件的面上
                //step3：获得原点的对应点，组成一条映射直线
                //step4：判断偏移方向
                //step5：计算引线的长度带来的影响
                //step6：计算宽度带来的折线路径
                Point3d ptProjectMP = currentFace.Part.GetPartPointPositionByNumber(currentFace.Part.MachinePoint.PointNumber);
                ptProjectMP = ptProjectMP.TransformBy(matrix);

                Point3d pt1 = currentFace.Point1.TransformBy(matrix);
                Point3d pt2 = currentFace.Point2.TransformBy(matrix);
                Point3d pt3 = currentFace.Point3.TransformBy(matrix);
                Point3d pt4 = currentFace.Point4.TransformBy(matrix);

                Point3d ptProjectMP2;
                Point3d ptProjectNormal;
                if (IsPointEqual(ptProjectMP, pt1))
                {
                    ptProjectMP2 = pt4;
                    ptProjectNormal = pt2;
                }
                else if (IsPointEqual(ptProjectMP, pt2))
                {
                    ptProjectMP2 = pt3;
                    ptProjectNormal = pt1;
                }
                else if (IsPointEqual(ptProjectMP, pt3))
                {
                    ptProjectMP2 = pt2;
                    ptProjectNormal = pt4;
                }
                else
                {
                    ptProjectMP2 = pt1;
                    ptProjectNormal = pt3;
                }

                Point3d ptLine1 = new Point3d(ptProjectMP.X, ptProjectMP.Y, this.Depth);
                Point3d ptLine2 = new Point3d(ptProjectMP2.X, ptProjectMP2.Y, this.Depth);
                List<Point3d> points = new List<Point3d>() { ptLine1, ptLine2 };

                if (this.ReverseDirection)//反向
                {
                    if (points[0].Y != points[1].Y)
                    {
                        points = points.OrderBy(it => it.Y).ToList();//只有两个点的时候可以这样！
                    }
                    else
                    {
                        points = points.OrderBy(it => it.X).ToList();//只有两个点的时候可以这样！
                    }
                }
                else
                {
                    if (points[0].Y != points[1].Y)
                    {
                        points = points.OrderBy(it => -it.Y).ToList();//只有两个点的时候可以这样！
                    }
                    else
                    {
                        points = points.OrderBy(it => -it.X).ToList();//只有两个点的时候可以这样！
                    }
                }


                //计算引线、长度、角度推算实际的路径
                double lengthTemp = points[0].DistanceTo(points[1]);
                double cos = (points[1].X - points[0].X) / lengthTemp;
                double sin = (points[1].Y - points[0].Y) / lengthTemp;
                Point3d ptStart = new Point3d(points[0].X - LeadIn * cos, points[0].Y - LeadIn * sin, this.Depth);
                Point3d ptEnd = new Point3d(points[1].X + LeadOut * cos, points[1].Y + LeadOut * sin, this.Depth);

                //排序之后再计算刀补的方向
                ToolComp comp = getToolComp(points[0], points[1], ptProjectNormal, f.FaceNumber);

                //计算折线的次数
                int count;
                if (DADOThick == 0 || DADOThick <= tool.Diameter)
                { count = 1; }
                else
                {
                    count = (int)System.Math.Ceiling(DADOThick / tool.Diameter);
                }

                double dist1 = tool.Diameter / 2;
                if (comp == ToolComp.Left)
                {
                    dist1 = -dist1;
                }

                Point3d ptXXX1 = ptStart;
                Point3d ptXXX2 = ptEnd;
                getOffsetPoints(dist1, ptXXX1, ptXXX2, out ptXXX1, out ptXXX2);

                List<Point3d> pointList = new List<Point3d>();
                pointList.Add(ptXXX1);
                pointList.Add(ptXXX2);

                for (int i = 1; i < count; i++)
                {
                    double dist;
                    if (i == count - 1)
                    {
                        dist = this.DADOThick - (tool.Diameter) * i;
                    }
                    else
                    {
                        dist = tool.Diameter;
                    }

                    if (i % 2 == 1)
                    {
                        if (comp == ToolComp.Right)
                        {
                            dist = -dist;
                        }
                    }
                    else
                    {
                        if (comp == ToolComp.Left)
                        {
                            dist = -dist;
                        }
                    }

                    getOffsetPoints(dist, ptXXX1, ptXXX2, out ptXXX1, out ptXXX2);
                    pointList.Add(ptXXX1);
                    pointList.Add(ptXXX2);
                }

                List<double> bulges = new List<double>();
                pointList.ForEach(it => bulges.Add(0));

                Routing route = new Routing();
                route.Bulges = bulges;
                route.Points = pointList;
                route.ToolComp = ToolComp.None;
                route.Part = f.Part;
                route.OnFace5 = (f.FaceNumber == 5) ? true : false;
                route.ToolName = this.ToolName;

                f.Part.Routings.Add(route);
            }
        }

        private void getOffsetPoints(double offsetDist, Point3d ptStart, Point3d ptEnd, out Point3d ptXXX1, out Point3d ptXXX2)
        {
            using (Line line = new Line(ptStart, ptEnd))
            using (DBObjectCollection lines = line.GetOffsetCurves(offsetDist))
            {
                if (lines.Count == 0 || lines.Count > 1)
                {
                    throw new Exception("Wrong offset data when calculating DADO thickness");
                }

                using (Line l = lines[0] as Line)
                {
                    ptXXX2 = l.StartPoint;
                    ptXXX1 = l.EndPoint;
                }
            }
        }

        private ToolComp getToolComp(Point3d ptProjectMP, Point3d ptProjectMP2, Point3d ptProjectNormal, int faceNumber)
        {
            ToolComp comp = ToolComp.None;
            double factor = (ptProjectMP2.X - ptProjectMP.X) * (ptProjectNormal.Y - ptProjectMP.Y) - (ptProjectNormal.X - ptProjectMP.X) * (ptProjectMP2.Y - ptProjectMP.Y);
            if (factor > 0)
            {
                comp = ToolComp.Right;
            }
            else if (factor < 0)
            {
                comp = ToolComp.Left;
            }
            else { throw new Exception("Wrong point when calculating Toolcomp for DADO!"); }
            return comp;
        }

        private bool IsPointEqual(Point3d ptProjectMP, Point3d pt1)
        {
            return System.Math.Abs(pt1.X - ptProjectMP.X) <= 0.01 && System.Math.Abs(pt1.Y - ptProjectMP.Y) <= 0.01;
        }
    }
}
