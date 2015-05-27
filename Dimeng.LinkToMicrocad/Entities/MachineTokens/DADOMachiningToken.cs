using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Geometry;
//using Dimeng.WoodEngine.Data.Job.Product.Machinings;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class DADOMachiningToken : BaseToken
    {
        public DADOMachiningToken(string token, string par1, string par2, string par3, string par4, string par5, string par6, string par7, string par8, string par9, int row, int column, Part p)
            : base(token, par1, par2, par3, par4, par5, par6, par7, par8, par9, row, column, p)
        {
            IsDrawOnly = false;
        }

        public override bool Valid(Logger logger)
        {
            this.logger = logger;

            base.faceNumberChecker(this.Token, 4, new int[] { 1, 2, 3, 4 });
            LeadIn = (string.IsNullOrEmpty(Par1)) ? 0 : base.DoubleChecker(Par1, "DADO/进刀引线长度", true);
            ReverseDirection = base.BoolChecker(Par2, "DADO/反向加工", false, false);
            Depth = base.DoubleChecker(Par3, "DADO/加工深度", true);
            LeadOut = (string.IsNullOrEmpty(Par4)) ? 0 : base.DoubleChecker(Par4, "DADO/退刀引线长度", true);
            DADOThick = base.DoubleChecker(Par5, "DADO/槽宽", true);
            ToolName = base.notEmptyStringChecker(Par7, "DADO/刀具名称");
            Penetration = base.DoubleChecker(Par8, "DADO/背板与槽间隙", false);

            return this.IsValid;
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

        public override void ToMachining(double AssociatedDist, ToolFile toolFile)
        {
            //得到当前面的关联面
            AssociatedPartFaceList = base.FindAssociatedFacesOneTime(AssociatedDist, Penetration);

            //当前的face
            var currentFace = this.Part.GetPartFaceByNumber(this.FaceNumber);

            foreach (var f in AssociatedPartFaceList)
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

                //将关联面的两个点作为铣型的起始
                var pt1 = currentFace.Point1.TransformBy(matrix);
                var pt2 = currentFace.Point4.TransformBy(matrix);


                //TODO:需要考虑的事情
                //1、LeadIn和LeadOut
                //2、铣型的方向
                //3、double/triple pass
                //4、刀补的方向如何偏移
                //5、宽度的方向如何偏移


                List<Point3d> points = new List<Point3d>() { pt1, pt2 };
                List<double> bulges = new List<double>() { 0, 0 };
                Machinings.Routing route = new Machinings.Routing();
                route.Bulges = bulges;
                route.Points = points;
                route.ToolComp = ToolComp.None;
                route.Part = f.Part;
                route.OnFace5 = (f.FaceNumber == 5) ? true : false;
                route.ToolName = this.ToolName;

                f.Part.Routings.Add(route);
            }
        }
    }
}
