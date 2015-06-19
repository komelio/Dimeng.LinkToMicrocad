using Autodesk.AutoCAD.Geometry;
using Dimeng.WoodEngine.Entities.MachineTokens;
using Dimeng.WoodEngine.Entities.Machinings;
using Dimeng.WoodEngine.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities
{
    public partial class Part
    {
        private Part()
        {
            MachineTokens = new List<BaseToken>();
            VDrillings = new List<VDrilling>();
            HDrillings = new List<HDrilling>();
            Sawings = new List<Sawing>();
            Routings = new List<Routing>();
            Faces = new List<PartFace>();
            Profiles = new List<Profile>();

            IsBend = false;
        }

        public Part(string partname,
                    int qty,
                    double width,
                    double length,
                    double thickness,
                    Material material,
                    EdgeBanding ebw1,
                    EdgeBanding ebw2,
                    EdgeBanding ebl1,
                    EdgeBanding ebl2,
                    string comment,
                    string comment2,
                    string comment3,
                    int bp,
                    MachinePoint mp,
                    double xo,
                    double yo,
                    double zo,
                    double xr,
                    double yr,
                    double zr,
                    string layer3DName,
                    string layer2DName,
                    bool isDraw3d,
                    IProduct product)
            : this()
        {
            this.PartName = partname;
            this.Width = width;
            this.Length = length;
            this.Qty = qty;
            this.Thickness = thickness;
            this.Material = material;
            this.EBW1 = ebw1;
            this.EBW2 = ebw2;
            this.EBL1 = ebl1;
            this.EBL2 = ebl2;
            this.Comment = comment;
            this.Comment2 = comment2;
            this.Comment3 = comment3;
            this.BasePoint = bp;
            this.MachinePoint = mp;
            this.XOrigin = xo;
            this.YOrigin = yo;
            this.ZOrigin = zo;
            this.XRotation = xr;
            this.YRotation = yr;
            this.ZRotation = zr;
            this.LayerName3D = layer3DName;
            this.LayerName2D = layer2DName;
            this.IsDrawOn3D = isDraw3d;

            this.Parent = product;

            this.TXOrigin = this.XOrigin;
            this.TYOrigin = this.YOrigin;
            this.TZOrigin = this.ZOrigin;
            this.TXRotation = this.XRotation;
            this.TYRotation = this.YRotation;
            this.TZRotation = this.ZRotation;

            calculateCutSize();
        }

        private void calculateCutSize()
        {
            this.CutWidth = this.Width - this.EBL1.Thickness - this.EBL2.Thickness;
            this.CutLength = this.Length - this.EBW1.Thickness - this.EBW2.Thickness;
        }

        private Product _product;
        public Product Product
        {
            get { return _product; }
            set
            {
                if (value == null)
                {
                    throw new Exception("Part`s product can not be null");
                }
                _product = value;
            }
        }
        private IProduct _parent;
        public IProduct Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                if (value is Product)
                {
                    this.Product = value as Product;
                }
                else if (value is Subassembly)
                {
                    Subassembly sub = value as Subassembly;

                    if (sub.Parent is Product)
                    {
                        this.Product = sub.Parent as Product;
                    }
                    else if (sub.Parent is Subassembly)
                    {
                        if (sub.Parent.Parent is Product)
                        {
                            this.Product = sub.Parent.Parent as Product;
                        }
                        else
                        {
                            throw new Exception("Wrong product type!");
                        }
                    }
                }
            }
        }

        public string PartName { get; set; }
        public int Qty { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }
        public double CutWidth { get; private set; }
        public double CutLength { get; private set; }
        public Material Material { get; set; }
        public double Thickness { get; set; }
        public EdgeBanding EBW1 { get; set; }
        public EdgeBanding EBW2 { get; set; }
        public EdgeBanding EBL1 { get; set; }
        public EdgeBanding EBL2 { get; set; }
        public string Comment { get; set; }
        public string Comment2 { get; set; }
        public string Comment3 { get; set; }

        public int BasePoint { get; set; }
        public MachinePoint MachinePoint { get; set; }

        //建模的坐标及旋转
        public double XOrigin { get; set; }
        public double YOrigin { get; set; }
        public double ZOrigin { get; set; }
        public double XRotation { get; set; }
        public double YRotation { get; set; }
        public double ZRotation { get; set; }

        public bool IsDrawOn3D { get; set; }

        public string LayerName2D { get; set; }
        public string LayerName3D { get; set; }
        public bool IsMirrorPart { get; set; }

        public List<BaseToken> MachineTokens { get; set; }
        public List<VDrilling> VDrillings { get; private set; }
        public List<HDrilling> HDrillings { get; private set; }
        public List<Routing> Routings { get; private set; }
        public List<Sawing> Sawings { get; private set; }
        public List<Profile> Profiles { get; private set; }

        #region 与空间定位相关的内容
        /// <summary>
        /// 板件中心向量,用于绘图时的旋转
        /// </summary>
        public Vector3d CenterVector { get; set; }

        /// <summary>
        /// 未进行任何旋转位移下MP的坐标
        /// </summary>
        public Point3d MPPoint { get; set; }
        /// <summary>
        /// 未进行任何旋转位移下MP的X向量
        /// </summary>
        public Vector3d MPXAxis { get; set; }
        /// <summary>
        /// 未进行任何旋转位移下MP的Y向量
        /// </summary>
        public Vector3d MPYAxis { get; set; }
        /// <summary>
        /// 未进行任何旋转位移下MP的Z向量
        /// </summary>
        public Vector3d MPZAxis { get; set; }
        /// <summary>
        /// 空间位移后的MP的坐标
        /// </summary>
        public Point3d MovedMPPoint { get; set; }
        /// <summary>
        /// 空间变化后的MP的X的向量
        /// </summary>
        public Vector3d MovedMPXAxis { get; set; }
        /// <summary>
        /// 空间变化后的MP的X的向量
        /// </summary>
        public Vector3d MovedMPYAxis { get; set; }
        /// <summary>
        /// 空间变化后的MP的X的向量
        /// </summary>
        public Vector3d MovedMPZAxis { get; set; }
        /// <summary>
        /// 基本向量，和任何点都没有关系，经过空间变化后的向量
        /// </summary>
        public Vector3d MovedOrginXAxis { get; private set; }
        public Vector3d MovedOrginYAxis { get; private set; }
        public Vector3d MovedOrginZAxis { get; private set; }
        #endregion

        public override string ToString()
        {
            return string.Format("{0}/{1}/{2}/{3}", PartName, Material, Width, Length);
        }



        public bool IsBend { get; set; }//是否为弧形板件，如果是的话，在生成机加工、绘制板件时都是不同的

        public bool IsValid { get; set; }

        public BendingInfo BendingInfo { get; set; }
    }
}
