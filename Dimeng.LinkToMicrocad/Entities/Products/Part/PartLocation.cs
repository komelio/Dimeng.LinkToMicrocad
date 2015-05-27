using Autodesk.AutoCAD.Geometry;
using Dimeng.WoodEngine.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities
{
    public partial class Part
    {
        public void CalculateLocationInfo(Point3d basepoint, double zr)
        {
            //获取中心向量的位置
            SetCenterVector(basepoint, zr);

            //设置MachinePoint相关的点和向量，与绘图有关
            SetMPPointAndAxis();

            //获取8个点的坐标
            SetPoints();

            //相关面的操作
            SetFaces();

            //设置最基本的向量位置
            SetOrginalAxis();
        }
        private void SetFaces()
        {
            Point3d p1 = Point3d.Origin;
            Point3d p2 = Point3d.Origin;
            Point3d p3 = Point3d.Origin;
            Point3d p4 = Point3d.Origin;
            Point3d p5 = Point3d.Origin;
            Point3d p6 = Point3d.Origin;
            Point3d p7 = Point3d.Origin;
            Point3d p8 = Point3d.Origin;
            if (!this.MachinePoint.IsRotated)
            {
                p1 = new Point3d(0, 0, 0);
                p2 = new Point3d(0, 0, Thickness);
                p3 = new Point3d(0, Width, 0);
                p4 = new Point3d(0, Width, Thickness);
                p5 = new Point3d(Length, Width, 0);
                p6 = new Point3d(Length, Width, Thickness);
                p7 = new Point3d(Length, 0, 0);
                p8 = new Point3d(Length, 0, Thickness);
            }
            else
            {
                p1 = new Point3d(0, 0, 0);
                p2 = new Point3d(0, 0, Thickness);
                p3 = new Point3d(0, Length, 0);
                p4 = new Point3d(0, Length, Thickness);
                p5 = new Point3d(Width, Length, 0);
                p6 = new Point3d(Width, Length, Thickness);
                p7 = new Point3d(Width, 0, 0);
                p8 = new Point3d(Width, 0, Thickness);
            }

            //将p1-p8从MP坐标系转换成为世界坐标系下的坐标
            p1 = p1.TransformBy(Matrix3d.AlignCoordinateSystem(new Point3d(), Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, MPPoint, MPXAxis, MPYAxis, MPZAxis));
            p2 = p2.TransformBy(Matrix3d.AlignCoordinateSystem(new Point3d(), Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, MPPoint, MPXAxis, MPYAxis, MPZAxis));
            p3 = p3.TransformBy(Matrix3d.AlignCoordinateSystem(new Point3d(), Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, MPPoint, MPXAxis, MPYAxis, MPZAxis));
            p4 = p4.TransformBy(Matrix3d.AlignCoordinateSystem(new Point3d(), Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, MPPoint, MPXAxis, MPYAxis, MPZAxis));
            p5 = p5.TransformBy(Matrix3d.AlignCoordinateSystem(new Point3d(), Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, MPPoint, MPXAxis, MPYAxis, MPZAxis));
            p6 = p6.TransformBy(Matrix3d.AlignCoordinateSystem(new Point3d(), Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, MPPoint, MPXAxis, MPYAxis, MPZAxis));
            p7 = p7.TransformBy(Matrix3d.AlignCoordinateSystem(new Point3d(), Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, MPPoint, MPXAxis, MPYAxis, MPZAxis));
            p8 = p8.TransformBy(Matrix3d.AlignCoordinateSystem(new Point3d(), Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, MPPoint, MPXAxis, MPYAxis, MPZAxis));
            p1 = MathHelper.GetRotatedAndMovedPoint(p1, TXRotation, TYRotation, TZRotation, CenterVector);
            p2 = MathHelper.GetRotatedAndMovedPoint(p2, TXRotation, TYRotation, TZRotation, CenterVector);
            p3 = MathHelper.GetRotatedAndMovedPoint(p3, TXRotation, TYRotation, TZRotation, CenterVector);
            p4 = MathHelper.GetRotatedAndMovedPoint(p4, TXRotation, TYRotation, TZRotation, CenterVector);
            p5 = MathHelper.GetRotatedAndMovedPoint(p5, TXRotation, TYRotation, TZRotation, CenterVector);
            p6 = MathHelper.GetRotatedAndMovedPoint(p6, TXRotation, TYRotation, TZRotation, CenterVector);
            p7 = MathHelper.GetRotatedAndMovedPoint(p7, TXRotation, TYRotation, TZRotation, CenterVector);
            p8 = MathHelper.GetRotatedAndMovedPoint(p8, TXRotation, TYRotation, TZRotation, CenterVector);

            //注意这些点的顺序，按顺时针或逆时针，不要乱序
            FaceOne = new PartFace(p1, p2, p8, p7, (p3 - p1).GetNormal(), 1, this);
            FaceTwo = new PartFace(p3, p4, p6, p5, (p1 - p3).GetNormal(), 2, this);
            FaceThree = new PartFace(p1, p2, p4, p3, (p7 - p1).GetNormal(), 3, this);
            FaceFour = new PartFace(p5, p6, p8, p7, (p1 - p7).GetNormal(), 4, this);
            FaceFive = new PartFace(p1, p3, p5, p7, (p2 - p1).GetNormal(), 5, this);
            FaceSix = new PartFace(p2, p4, p6, p8, (p1 - p2).GetNormal(), 6, this);

            Faces.Clear();
            Faces.Add(FaceOne);
            Faces.Add(FaceTwo);
            Faces.Add(FaceThree);
            Faces.Add(FaceFour);
            Faces.Add(FaceFive);
            Faces.Add(FaceSix);
        }
        private void SetPoints()
        {
            Point1 = new Point3d(-Length / 2, -Width / 2, -Thickness / 2);
            Point2 = new Point3d(-Length / 2, -Width / 2, Thickness / 2);
            Point3 = new Point3d(-Length / 2, Width / 2, -Thickness / 2);
            Point4 = new Point3d(-Length / 2, Width / 2, Thickness / 2);
            Point5 = new Point3d(Length / 2, Width / 2, -Thickness / 2);
            Point6 = new Point3d(Length / 2, Width / 2, Thickness / 2);
            Point7 = new Point3d(Length / 2, -Width / 2, -Thickness / 2);
            Point8 = new Point3d(Length / 2, -Width / 2, Thickness / 2);
            Point1 = MathHelper.GetRotatedAndMovedPoint(Point1, TXRotation, TYRotation, TZRotation, CenterVector);
            Point2 = MathHelper.GetRotatedAndMovedPoint(Point2, TXRotation, TYRotation, TZRotation, CenterVector);
            Point3 = MathHelper.GetRotatedAndMovedPoint(Point3, TXRotation, TYRotation, TZRotation, CenterVector);
            Point4 = MathHelper.GetRotatedAndMovedPoint(Point4, TXRotation, TYRotation, TZRotation, CenterVector);
            Point5 = MathHelper.GetRotatedAndMovedPoint(Point5, TXRotation, TYRotation, TZRotation, CenterVector);
            Point6 = MathHelper.GetRotatedAndMovedPoint(Point6, TXRotation, TYRotation, TZRotation, CenterVector);
            Point7 = MathHelper.GetRotatedAndMovedPoint(Point7, TXRotation, TYRotation, TZRotation, CenterVector);
            Point8 = MathHelper.GetRotatedAndMovedPoint(Point8, TXRotation, TYRotation, TZRotation, CenterVector);
        }
        private void SetMPPointAndAxis()
        {
            MPPoint = GetPartPointByNumber(MachinePoint.MP);
            MPXAxis = GetMPXAxis(MachinePoint.MP);
            MPYAxis = GetMPYAxis(MachinePoint.MP);
            MPZAxis = GetMPZAxis(MachinePoint.MP);

            MovedMPPoint = MathHelper.GetRotatedAndMovedPoint(MPPoint, TXRotation, TYRotation, TZRotation, CenterVector);
            MovedMPXAxis = MathHelper.GetRotateVector(MPXAxis, TXRotation, TYRotation, TZRotation);
            MovedMPYAxis = MathHelper.GetRotateVector(MPYAxis, TXRotation, TYRotation, TZRotation);
            MovedMPZAxis = MathHelper.GetRotateVector(MPZAxis, TXRotation, TYRotation, TZRotation);
        }
        private void SetOrginalAxis()
        {
            MovedOrginXAxis = MathHelper.GetRotateVector(Vector3d.XAxis, TXRotation, TYRotation, TZRotation);
            MovedOrginYAxis = MathHelper.GetRotateVector(-Vector3d.YAxis, TXRotation, TYRotation, TZRotation);
            MovedOrginZAxis = MathHelper.GetRotateVector(Vector3d.ZAxis, TXRotation, TYRotation, TZRotation);
        }
        public Point3d GetPartPointByNumber(string num)
        {
            if (num == "1" || num == "1M")
                return new Point3d(-Length / 2, -Width / 2, -Thickness / 2);
            else if (num == "2" || num == "2M")
                return new Point3d(-Length / 2, -Width / 2, Thickness / 2);
            else if (num == "3" || num == "3M")
                return new Point3d(-Length / 2, Width / 2, -Thickness / 2);
            else if (num == "4" || num == "4M")
                return new Point3d(-Length / 2, Width / 2, Thickness / 2);
            else if (num == "5" || num == "5M")
                return new Point3d(Length / 2, Width / 2, -Thickness / 2);
            else if (num == "6" || num == "6M")
                return new Point3d(Length / 2, Width / 2, Thickness / 2);
            else if (num == "7" || num == "7M")
                return new Point3d(Length / 2, -Width / 2, -Thickness / 2);
            else if (num == "8" || num == "8M")
                return new Point3d(Length / 2, -Width / 2, Thickness / 2);

            throw new System.Exception("Error Machinepoint!" + MachinePoint.MP);
        }
        public Vector3d GetMPXAxis(string mp)
        {
            if (mp == "1" || mp == "4" || mp == "2M" || mp == "3M") return Vector3d.XAxis;
            else if (mp == "7M" || mp == "8" || mp == "6M" || mp == "5") return -Vector3d.XAxis;
            else if (mp == "1M" || mp == "8M" || mp == "2" || mp == "7") return Vector3d.YAxis;
            else return -Vector3d.YAxis;
        }
        public Vector3d GetMPYAxis(string mp)
        {
            if (mp == "1" || mp == "8" || mp == "2M" || mp == "7M") return Vector3d.YAxis;
            else if (mp == "3M" || mp == "4" || mp == "6M" || mp == "5") return -Vector3d.YAxis;
            else if (mp == "1M" || mp == "4M" || mp == "2" || mp == "3") return Vector3d.XAxis;
            else return -Vector3d.XAxis;
        }
        public Vector3d GetMPZAxis(string mp)
        {
            if (mp == "1" || mp == "1M" || mp == "3" || mp == "3M" || mp == "5" || mp == "5M" || mp == "7" || mp == "7M") return Vector3d.ZAxis;
            else return -Vector3d.ZAxis;
        }
        private void SetCenterVector(Point3d OriginPoint, double zr)
        {
            this.TZRotation += zr;

            Point3d position = new Point3d(this.XOrigin, this.YOrigin, this.ZOrigin);
            position = (OriginPoint == Point3d.Origin) ? position :
                                     position.TransformBy(Matrix3d.Displacement(new Vector3d(OriginPoint.X, OriginPoint.Y, OriginPoint.Z)));
            position = position.TransformBy(Matrix3d.Rotation(zr / 180 * System.Math.PI, Vector3d.ZAxis, OriginPoint));

            this.TXOrigin = position.X;
            this.TYOrigin = position.Y;
            this.TZOrigin = position.Z;

            Vector3d BaseVector = new Vector3d();//定位向量
            if (BasePoint == 1)
                BaseVector = new Vector3d(-Length / 2, -Width / 2, -Thickness / 2);
            else if (BasePoint == 2)
                BaseVector = new Vector3d(-Length / 2, -Width / 2, Thickness / 2);
            else if (BasePoint == 3)
                BaseVector = new Vector3d(-Length / 2, Width / 2, -Thickness / 2);
            else if (BasePoint == 4)
                BaseVector = new Vector3d(-Length / 2, Width / 2, Thickness / 2);
            else if (BasePoint == 5)
                BaseVector = new Vector3d(Length / 2, Width / 2, -Thickness / 2);
            else if (BasePoint == 6)
                BaseVector = new Vector3d(Length / 2, Width / 2, Thickness / 2);
            else if (BasePoint == 7)
                BaseVector = new Vector3d(Length / 2, -Width / 2, -Thickness / 2);
            else if (BasePoint == 8)
                BaseVector = new Vector3d(Length / 2, -Width / 2, Thickness / 2);
            else throw new System.Exception("Unknown Basepoint" + BasePoint.ToString());

            Vector3d MoveVector = MathHelper.GetRotateVector(BaseVector, TXRotation, TYRotation, TZRotation);
            Vector3d DimVector = new Vector3d(TXOrigin, TYOrigin, TZOrigin);

            CenterVector = -MoveVector + DimVector;
        }

        //对应实际的坐标及旋转,如组件中的元素，经过各种旋转转换后的结果
        public double TXOrigin { get; set; }
        public double TYOrigin { get; set; }
        public double TZOrigin { get; set; }
        public double TXRotation { get; set; }
        public double TYRotation { get; set; }
        public double TZRotation { get; set; }

        //板件的8个点，在空间的最终坐标 （即经过各种旋转、位移后的），数字的定义遵循MV的板件角点定义
        public Point3d Point1 { get; set; }
        public Point3d Point2 { get; set; }
        public Point3d Point3 { get; set; }
        public Point3d Point4 { get; set; }
        public Point3d Point5 { get; set; }
        public Point3d Point6 { get; set; }
        public Point3d Point7 { get; set; }
        public Point3d Point8 { get; set; }

        //板件的6个面
        public PartFace FaceOne = new PartFace();
        public PartFace FaceTwo = new PartFace();
        public PartFace FaceThree = new PartFace();
        public PartFace FaceFive = new PartFace();
        public PartFace FaceFour = new PartFace();
        public PartFace FaceSix = new PartFace();
       
        public PartFace GetPartFaceByNumber(int num)
        {
            switch (num)
            {
                case 1:
                    return FaceOne;
                case 2:
                    return FaceTwo;
                case 3:
                    return FaceThree;
                case 4:
                    return FaceFour;
                case 5:
                    return FaceFive;
                case 6:
                    return FaceSix;
                default:
                    throw new System.Exception("你所要的面" + num.ToString() + "是非法的");
            }
        }
        
        public List<PartFace> Faces { get; private set; }

        public Point3d GetPartPointPositionByNumber(int num)
        {
            switch (num)
            {
                case 1:
                    return Point1;
                case 2:
                    return Point2;
                case 3:
                    return Point3;
                case 4:
                    return Point4;
                case 5:
                    return Point5;
                case 6:
                    return Point6;
                case 7:
                    return Point7;
                case 8:
                    return Point8;
                default:
                    return Point1;
            }
        }
    }
}
