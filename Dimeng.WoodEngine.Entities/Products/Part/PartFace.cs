﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.DatabaseServices;

namespace Dimeng.WoodEngine.Entities
{
    public class PartFace
    {
        public Vector3d Normal { get; private set; }
        public int FaceNumber { get; set; }
        public Part Part { get; set; }//面所属的板件

        public PartFace()
        { }

        /// <summary>
        /// 四个点的顺序需要注意，要顺时针，并且保证1和4组成非厚度边，影响后续的机加工的判断
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <param name="number"></param>
        /// <param name="p"></param>
        public PartFace(Point3d p1, Point3d p2, Point3d p3, Point3d p4, Vector3d normal, int number, Part p)
        {
            Point1 = p1;
            Point2 = p2;
            Point3 = p3;
            Point4 = p4;
            this.Normal = normal;
            FaceNumber = number;
            Part = p;
            Plane = new Plane(p1, p2, p3);
            Points.Add(Point1);
            Points.Add(Point2);
            Points.Add(Point3);
            Points.Add(Point4);

            if (FaceNumber <= 4)
            { IsHorizontalFace = true; }
            else { IsHorizontalFace = false; }
        }

        /// <summary>
        /// 析构函数，销毁Plane
        /// </summary>
        ~PartFace()
        {
            if (this.Plane != null)
            {
                Plane.Dispose();
            }
        }

        //四个端点，最后表现为世界坐标系之下
        public Point3d Point1 { get; set; }
        public Point3d Point2 { get; set; }
        public Point3d Point3 { get; set; }
        public Point3d Point4 { get; set; }
        private List<Point3d> Points = new List<Point3d>();

        public Plane Plane { get; set; }//面所在的CAD平面，用于一些面的关系的判断，如平行、距离

        public bool IsAssocaitedWithAnotherFace(PartFace anotherFace, double associateDist, double tolerenceDist)
        {
            /*
             * 判断原则
             * 1、两个面平行且距离小于某个固定值
             * 2A、指令面的所有顶点，投影到被关联面，至少有一个点位于关联面的内部
             * 2B、（改进算法）指令面的两个对角点，如果投影后都在另一个面内部，则认为是关联的，
             *     如果不是都在则有可能一个或两个都不在内部
             *     做一条线段链接起来，和另一面的四条边做交点
             *     如果有一个交点则是关联的
             * 3、如果是开槽关联，则上述两条不适用
             */

            //如果他们是同一个板件的不同面，永远都是不关联的
            if (this.Part == anotherFace.Part)
                return false;

            //如果两个面不平行，那也是不关联的
            if (!anotherFace.Plane.IsParallelTo(this.Plane))
            {
                return false;
            }

            //两个面的法向量一样，也不相关联
            if (anotherFace.Normal.IsEqualTo(this.Normal, Tolerance.Global))
            {
                return false;
            }

            //z值要刚好位于设定的位置上
            if (!anotherFace.IsPointZFit(this.Point1, associateDist, tolerenceDist))
            {
                return false;
            }

            ////如果两个面的距离大于一定距离也就不关联
            //if (anotherFace.Plane.DistanceTo(this.Point1) > dist)
            //{
            //    return false;
            //}

            //计算所在面的四个点，共有几个点落在另一个面的板件范围内
            int PtInPartsCount = this.Points.Count(pt => anotherFace.IsPointIn(pt, associateDist, tolerenceDist));
            //对于所关联的面是面5、面6或者是4个边，对点的数量要求也不同
            if (anotherFace.IsHorizontalFace && PtInPartsCount == 4)
            {
                return true;
            }
            else if (!anotherFace.IsHorizontalFace)
            {
                if (PtInPartsCount <= 2)
                {
                    //对于没有一个点在内部的情况，要判断对角线与板件四边的投影线是否有交点
                    return anotherFace.IsLineCrossBorder(this.Point1, this.Point3);
                }
                else
                {
                    //三个以上的，就必然了
                    return true;
                }
            }

            return false;
        }

        public bool IsHorizontalFace { get; set; }

        public bool IsPointZFit(Point3d pt, double associateDist, double tolerenceDist)
        {
            if (tolerenceDist < 0)
            {
                throw new Exception("Tolerence dist value could not below zero!");
            }

            //先把世界坐标系下的pt转换成mp坐标系下
            pt = pt.TransformBy(Matrix3d.AlignCoordinateSystem(this.Part.MovedMPPoint,
                                                               this.Part.MovedMPXAxis,
                                                               this.Part.MovedMPYAxis,
                                                               this.Part.MovedMPZAxis,
                                                               Point3d.Origin,
                                                               Vector3d.XAxis,
                                                               Vector3d.YAxis,
                                                               Vector3d.ZAxis));
            //取有效数字两个，是为了避免精确计算的情况下的误差导致判断错误
            double z = System.Math.Round(pt.Z, 2);

            double zlimitUp = 0, zlimitDown = 0;

            if (FaceNumber == 5)
            {
                zlimitUp = -associateDist + tolerenceDist;
                zlimitDown = -associateDist - tolerenceDist;
            }
            else if (FaceNumber == 6)
            {
                zlimitUp = this.Part.Thickness + associateDist + tolerenceDist;
                zlimitDown = this.Part.Thickness + associateDist - tolerenceDist;
            }
            //todo 面1-4怎么办

            //板件是否旋转，判断的区间也不同
            if (this.Part.MachinePoint.IsRotated)
            {
                if (z >= zlimitDown && z <= zlimitUp)
                { return true; }
            }
            else
            {
                if (z >= zlimitDown && z <= zlimitUp)
                { return true; }
            }
            return false;
        }
        /// <summary>
        /// 判断一个点是否在板件内部
        /// </summary>
        /// <param name="pt">世界坐标下的点坐标</param>
        /// <returns></returns>
        public bool IsPointIn(Point3d pt, double associateDist, double tolerenceDist)
        {
            if (tolerenceDist < 0)
            {
                throw new Exception("Tolerence dist value could not below zero!");
            }

            //先把世界坐标系下的pt转换成mp坐标系下
            pt = pt.TransformBy(Matrix3d.AlignCoordinateSystem(this.Part.MovedMPPoint,
                                                               this.Part.MovedMPXAxis,
                                                               this.Part.MovedMPYAxis,
                                                               this.Part.MovedMPZAxis,
                                                               Point3d.Origin,
                                                               Vector3d.XAxis,
                                                               Vector3d.YAxis,
                                                               Vector3d.ZAxis));
            //取有效数字两个，是为了避免精确计算的情况下的误差导致判断错误
            double x = System.Math.Round(pt.X, 2);
            double y = System.Math.Round(pt.Y, 2);
            double z = System.Math.Round(pt.Z, 2);

            //板件是否旋转，判断的区间也不同
            if (this.Part.MachinePoint.IsRotated)
            {
                if (x >= 0 && x <= this.Part.Width
                    && y >= 0 && y <= this.Part.Length)
                { return true; }
            }
            else
            {
                if (x >= 0 && x <= this.Part.Length
                    && y >= 0 && y <= this.Part.Width)
                { return true; }
            }
            return false;
        }

        /// <summary>
        /// 判断两点组成的线段是否和面的四个线段有交点
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <returns></returns>
        public bool IsLineCrossBorder(Point3d pt1, Point3d pt2)
        {
            pt1 = getProjectPoint(pt1);
            pt2 = getProjectPoint(pt2);

            pt1 = new Point3d(pt1.X, pt1.Y, 0);
            pt2 = new Point3d(pt2.X, pt2.Y, 0);

            using (Line line = new Line(pt1, pt2))
            {
                using (Line line1 = new Line(Point3d.Origin, new Point3d(this.Part.Length, 0, 0)))
                {
                    if (IsLinesInterestPoint(line, line1))
                        return true;
                }

                using (Line line2 = new Line(new Point3d(this.Part.Length, 0, 0), new Point3d(this.Part.Length, this.Part.Width, 0)))
                {
                    if (IsLinesInterestPoint(line, line2))
                        return true;
                }

                using (Line line3 = new Line(new Point3d(this.Part.Length, this.Part.Width, 0), new Point3d(0, this.Part.Width, 0)))
                {
                    if (IsLinesInterestPoint(line, line3))
                        return true;
                }

                using (Line line4 = new Line(new Point3d(0, this.Part.Width, 0), Point3d.Origin))
                {
                    if (IsLinesInterestPoint(line, line4))
                        return true;
                }

                return false;
            }
        }

        private bool IsLinesInterestPoint(Line line1, Line line2)
        {
            Point3dCollection pts = new Point3dCollection();
            line1.IntersectWith(line2, Intersect.OnBothOperands, pts, IntPtr.Zero, IntPtr.Zero);

            //完全没有交点，就肯定是false
            if (pts.Count == 0)
                return false;

            //有一个交点，必须不能是线的起点或终点
            if (pts.Count == 1 && (pts[0] == line1.StartPoint || pts[0] == line1.EndPoint))
            {
                return false;
            }

            //有两个交点

            return true;
        }

        /// <summary>
        /// 取得世界坐标系下的点在这个板件的MP坐标系下的坐标
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        private Point3d getProjectPoint(Point3d pt)
        {
            //先把世界坐标系下的pt转换成mp坐标系下
            pt = pt.TransformBy(Matrix3d.AlignCoordinateSystem(this.Part.MovedMPPoint,
                                                               this.Part.MovedMPXAxis,
                                                               this.Part.MovedMPYAxis,
                                                               this.Part.MovedMPZAxis,
                                                               Point3d.Origin,
                                                               Vector3d.XAxis,
                                                               Vector3d.YAxis,
                                                               Vector3d.ZAxis));
            //取有效数字两个，是为了避免精确计算的情况下的误差导致判断错误
            double x = System.Math.Round(pt.X, 2);
            double y = System.Math.Round(pt.Y, 2);
            double z = System.Math.Round(pt.Z, 2);

            return new Point3d(x, y, z);
        }

        private Point3d ReSetPointZ(Point3d pt, double z)
        {
            return new Point3d(pt.X, pt.Y, z);
        }

        /// <summary>
        /// 判断点是否嵌入了表面所在的板件
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="peakValue">嵌入板件的深度值，按照设计这个值应当是负值</param>
        /// <param name="associateDist">判定时的容差，为正值</param>
        /// <returns></returns>
        private bool IsDistEnough(PartFace anotherFace, Point3d pt, double peakValue, double associateDist)
        {
            //先把世界坐标系下的pt转换成mp坐标系下
            pt = pt.TransformBy(Matrix3d.AlignCoordinateSystem(anotherFace.Part.MovedMPPoint,
                                                               anotherFace.Part.MovedMPXAxis,
                                                               anotherFace.Part.MovedMPYAxis,
                                                               anotherFace.Part.MovedMPZAxis,
                                                               Point3d.Origin,
                                                               Vector3d.XAxis,
                                                               Vector3d.YAxis,
                                                               Vector3d.ZAxis));

            //取有效数字两个，是为了避免精确计算的情况下的误差导致判断错误
            double z = System.Math.Round(pt.Z, 2);

            //面5和面6的判断不同，这是因为Z的正方向发生了变化
            double zlimitDown = 0 - peakValue;
            double zlimitUp = associateDist - peakValue;

            if (anotherFace.FaceNumber == 6)
            {
                zlimitUp = associateDist + this.Part.Thickness + peakValue;
                zlimitDown = this.Part.Thickness + peakValue;
            }
            if (z >= zlimitDown && z <= zlimitUp)
                return true;
            else return false;
        }
    }
}
