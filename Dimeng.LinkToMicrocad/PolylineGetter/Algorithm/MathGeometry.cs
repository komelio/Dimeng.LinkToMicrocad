using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//对多线段进行几何计算类

namespace PolylineGetter.Algorithm
{
    //几何点
    class point
    {
        public double x, y;

        public point(double tmp_x, double tmp_y)
        {
            x = tmp_x;
            y = tmp_y;
        }

        public bool IsSamePoint(point pt)
        {
            return Math.Abs(pt.x - x) < 1e-6 && Math.Abs(pt.y - y) < 1e-6;
        }
    }

    //几何弧形
    class Hu
    {
        public point from, to;//首尾点
        public double h;//供高比
        public Hu(point tmp_from, point tmp_to, double tmp_h)
        {
            from = new point(tmp_from.x, tmp_from.y);
            to = new point(tmp_to.x, tmp_to.y);
            h = tmp_h;
        }
    }

    //几何圆
    class Circle
    {
        public point center;//圆心
        public double r;//半径
        public Circle(point tmp_center, double tmp_r)
        {
            center = new point(tmp_center.x, tmp_center.y);
            r = tmp_r;
        }
    }

    //几何处理基础类
    class MathGeometry
    {
        public static double myeps = 1.0f;

        //判断浮点数是否为0
        private static bool zero(double x)
        {
            return Math.Abs(x) < 1e-6;
        }

        //两点距离
        public static double distance(point p1, point p2)
        {
            try
            {
                return Math.Sqrt((p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y));
            }
            catch
            {
                throw;
            }
        }

        //求差积
        private static double xmult(point p1, point p2, point p0)
        {
            return (p1.x - p0.x) * (p2.y - p0.y) - (p2.x - p0.x) * (p1.y - p0.y);
        }

        //判断点p1和p2是否在直线l1-l2的两侧
        public static bool opposite_side(point p1, point p2, point l1, point l2)
        {
            return xmult(l1, p1, l2) * xmult(l1, p2, l2) < 1e-6;
        }

        //判断两直线平行
        public static bool parallel(point u1, point u2, point v1, point v2)
        {
            return zero((u1.x - u2.x) * (v1.y - v2.y) - (v1.x - v2.x) * (u1.y - u2.y));
        }

        //判两线段相交,不包括端点和部分重合
        private static bool intersect_ex(point u1, point u2, point v1, point v2)
        {
            return opposite_side(u1, u2, v1, v2) && opposite_side(v1, v2, u1, u2);
        }

        //判点p0是否在线段p1->p2上,包括端点
        public static bool dot_online_in(point p0, point p1, point p2)
        {
            return zero(xmult(p0, p1, p2)) && (p1.x - p0.x) * (p2.x - p0.x) < 1e-6 && (p1.y - p0.y) * (p2.y - p0.y) < 1e-6;
        }

        //求两直线交点
        public static point intersection_line(point u1, point u2, point v1, point v2)
        {
            if (true == parallel(u1, u2, v1, v2))//平行
            {
                return null;
            }
            point ret = new point(u1.x, u1.y);
            double t = ((u1.x - v1.x) * (v1.y - v2.y) - (u1.y - v1.y) * (v1.x - v2.x))
                    / ((u1.x - u2.x) * (v1.y - v2.y) - (u1.y - u2.y) * (v1.x - v2.x));
            ret.x += (u2.x - u1.x) * t;
            ret.y += (u2.y - u1.y) * t;

            return ret;
        }

        //求两线段交点
        public static point intersection_segment(point u1, point u2, point v1, point v2)
        {
            //如果平行而且重合
            //TODO：增加对部分重合的处理

            //直线交点
            point ret = intersection_line(u1, u2, v1, v2);
            if (null == ret)
            {
                return null;
            }

            //若不在线段上
            if (false == dot_online_in(ret, u1, u2)
                || false == dot_online_in(ret, v1, v2))
            {
                ret = null;
            }
            return ret;
        }

        //点到直线上的最近点
        private static point ptoline(point p, point l1, point l2)
        {
            point t = new point(p.x, p.y);
            t.x += l1.y - l2.y;
            t.y += l2.x - l1.x;
            return intersection_line(p, t, l1, l2);
        }

        //根据圆上三个点求圆心
        public static point getCenter(point p1, point p2, point p3)
        {
            point center = null;

            //虚拟出两个弧，分别为p1->p2，p2->p3， 供高比为1
            Hu virtual_hu_1 = new Hu(p1, p2, 1);
            Hu virtual_hu_2 = new Hu(p2, p3, 1);

            //虚拟弧的弧顶
            point huTop_1 = getHutop(virtual_hu_1);
            point huTop_2 = getHutop(virtual_hu_2);

            //虚拟弧的弦中点
            point mid_1 = new point((p1.x + p2.x) / 2, (p1.y + p2.y) / 2);
            point mid_2 = new point((p2.x + p3.x) / 2, (p2.y + p3.y) / 2);

            //两条虚拟弧的“弧顶->弦中点”连线的交点
            center = intersection_line(huTop_1, mid_1, huTop_2, mid_2);

            /*
            原理：过两个点的任意弧，他们的半径所在的直线都重合，所以虚拟一个弧出来即可
            */


            return center;
        }

        //直线上二分搜索寻找圆上的点，从点farPoint到点closePoint，到点center的距离越来越近，二分查找距离为r的点
        private static point binarySearch(point farPoint, point closePoint, double r, point center)
        {
            point low = closePoint;
            point high = farPoint;
            double distMax = distance(farPoint, center);
            double distMin = distance(closePoint, center);

            if (r - distMax > 1e-6
                || distMin - r > 1e-6)
            {
                return null;
            }

            while (distance(low, high) > 1e-6)
            {
                point mid = new point((low.x + high.x) / 2, (low.y + high.y) / 2);
                double curDist = distance(mid, center);
                if (curDist - r > 1e-6)//太远
                {
                    high = new point(mid.x, mid.y);
                }
                else if (r - curDist > 1e-6)//太近
                {
                    low = new point(mid.x, mid.y);
                }
                else//刚好距离为r
                {
                    return mid;
                }
            }
            return null;
        }

        //求弧顶坐标
        public static point getHutop(Hu hu)
        {
            point huTop = null;

            //弧两端坐标差值
            double detaX = Math.Abs(hu.to.x - hu.from.x);
            double detaY = Math.Abs(hu.to.y - hu.from.y);

            //角度sita：线段的倾角
            double sinValue = detaY / Math.Sqrt(detaX * detaX + detaY * detaY);
            double sita = Math.Abs(Math.Asin(sinValue));

            //供高
            double gongGao = Math.Abs(hu.h) * distance(hu.from, hu.to) / 2;

            //弦中点
            point midXian = new point((hu.from.x + hu.to.x) / 2, (hu.from.y + hu.to.y) / 2);
            //弧顶
            if (Math.Abs(detaX) < 1e-6)
            {
                if ((hu.h > 0 && hu.to.y > hu.from.y)
                    || (hu.h < 0 && hu.to.y < hu.from.y))
                {
                    huTop = new point(midXian.x - gongGao, midXian.y);
                }
                else
                {
                    huTop = new point(midXian.x + gongGao, midXian.y);
                }
            }
            else if (Math.Abs(detaY) < 1e-6)
            {
                if ((hu.h > 0 && hu.to.x > hu.from.x)
                    || (hu.h < 0 && hu.to.x < hu.from.x))
                {
                    huTop = new point(midXian.x, midXian.y + gongGao);
                }
                else
                {
                    huTop = new point(midXian.x, midXian.y - gongGao);
                }
            }
            else
            {
                double xFlag = 1;
                double yFlag = 1;
                if ((hu.h > 0 && hu.to.y > hu.from.y)
                    || (hu.h < 0 && hu.to.y < hu.from.y))
                {
                    xFlag = -1;
                }
                if ((hu.h > 0 && hu.to.x < hu.from.x)
                    || (hu.h < 0 && hu.to.x > hu.from.x))
                {
                    yFlag = -1;
                }
                //几何距离
                double distX = gongGao * Math.Sin(sita);
                double distY = gongGao * Math.Cos(sita);

                huTop = new point(midXian.x + xFlag * distX, midXian.y + yFlag * distY);
            }
            return huTop;
        }

        //求直线u1->u2与弧hu的交点
        public static List<point> intersection_line_hu(point u1, point u2, Hu hu)
        {
            List<point> lstPoint = new List<point>();

            //弧顶
            point huTop = getHutop(hu);

            //圆心
            point center = getCenter(hu.from, hu.to, huTop);

            //圆心到直线u1,u2上的最近点
            point closestToCenter = ptoline(center, u1, u2);

            //交点存在
            if (null != closestToCenter)
            {

                //半径
                double r = distance(center, huTop);

                //放大线段，保证线段与弧所在圆有交点
                point pt1 = new point(u1.x, u1.y);
                point pt2 = new point(u2.x, u2.y);
                //先在直线方向上，把点分到异侧
                while (false == opposite_side(pt1, pt2, center, closestToCenter))
                {
                    pt1.x += pt1.x - pt2.x;
                    pt1.y += pt1.y - pt2.y;
                    pt2.x += pt2.x - pt1.x;
                    pt2.y += pt2.y - pt1.y;
                }
                //然后让点远离圆心
                while (distance(pt1, center) < r)
                {
                    pt1.x += pt1.x - pt2.x;
                    pt1.y += pt1.y - pt2.y;
                }
                //然后让点远离圆心
                while (distance(pt2, center) < r)
                {
                    pt2.x += pt2.x - pt1.x;
                    pt2.y += pt2.y - pt1.y;
                }

                //输入线段与弧所在圆的交点
                point crossP0 = binarySearch(pt1, closestToCenter, r, center);
                point crossP1 = binarySearch(pt2, closestToCenter, r, center);

                //与弧顶同侧
                if (null != crossP0
                    && false == opposite_side(crossP0, huTop, hu.from, hu.to))
                {
                    lstPoint.Add(crossP0);
                }

                //与弧顶同侧
                if (null != crossP1
                    && false == opposite_side(crossP1, huTop, hu.from, hu.to))
                {
                    lstPoint.Add(crossP1);
                }
            }
            return lstPoint;
        }

        //求线段u1->u2与弧hu的交点
        public static List<point> intersection_segment_hu(point u1, point u2, Hu hu)
        {
            List<point> lstPoint = new List<point>();

            //弧顶
            point huTop = getHutop(hu);

            //圆心
            point center = getCenter(hu.from, hu.to, huTop);

            //圆心到直线u1,u2上的最近点
            point closestToCenter = ptoline(center, u1, u2);

            //交点存在
            if (null != closestToCenter)
            {

                //半径
                double r = distance(center, huTop);

                //输入线段所在直线，与弧所在圆的交点
                point crossP0 = binarySearch(u1, closestToCenter, r, center);
                point crossP1 = binarySearch(u2, closestToCenter, r, center);

                //在线段上且与弧顶同侧
                if (null != crossP0
                    && true == dot_online_in(crossP0, u1, u2)
                    && false == opposite_side(crossP0, huTop, hu.from, hu.to))
                {
                    lstPoint.Add(crossP0);
                }

                //在线段上且与弧顶同侧
                if (null != crossP1
                    && true == dot_online_in(crossP1, u1, u2)
                    && false == opposite_side(crossP1, huTop, hu.from, hu.to))
                {
                    lstPoint.Add(crossP1);
                }
            }
            return lstPoint;
        }

        //判断两个浮点数是否相等
        private static bool double_equals(double a, double b)
        {
            return zero(a - b);
        }

        //求两圆交点
        public static List<point> intersection_circle(Circle circle_1, Circle circle_2)
        {

            List<point> lstPoint = new List<point>();

            double cos_value_1, sin_value_1;
            double cos_value_2, sin_value_2;
            //相同的圆
            if (double_equals(circle_1.center.x, circle_2.center.x)
                && double_equals(circle_1.center.y, circle_2.center.y)
                && double_equals(circle_1.r, circle_2.r))
            {
                return lstPoint;
            }

            //圆心距离
            double d = distance(circle_1.center, circle_2.center);
            //相离的两圆
            if (d > circle_1.r + circle_2.r
                || d < Math.Abs(circle_1.r - circle_2.r))
            {
                return lstPoint;
            }

            double a = 2.0 * circle_1.r * (circle_1.center.x - circle_2.center.x);
            double b = 2.0 * circle_1.r * (circle_1.center.y - circle_2.center.y);
            double c = circle_2.r * circle_2.r - circle_1.r * circle_1.r - d * d;
            double p = a * a + b * b;
            double q = -2.0 * a * c;
            //相切的两圆
            if (double_equals(d, circle_1.r + circle_2.r)
                || double_equals(d, Math.Abs(circle_1.r - circle_2.r)))
            {
                cos_value_1 = -q / p / 2.0;
                sin_value_1 = Math.Sqrt(1 - cos_value_1 * cos_value_1);

                double x = circle_1.r * cos_value_1 + circle_1.center.x;
                double y = circle_1.r * sin_value_1 + circle_1.center.y;
                point crossPoint = new point(x, y);

                if (!double_equals(distance(crossPoint, circle_2.center), circle_2.r))
                {
                    crossPoint.y = circle_1.center.y - circle_1.r * sin_value_1;
                }
                lstPoint.Add(crossPoint);
                return lstPoint;
            }
            //相交的两圆
            else
            {
                double r = c * c - b * b;
                cos_value_1 = (Math.Sqrt(q * q - 4.0 * p * r) - q) / p / 2.0;
                cos_value_2 = (-Math.Sqrt(q * q - 4.0 * p * r) - q) / p / 2.0;
                sin_value_1 = Math.Sqrt(1 - cos_value_1 * cos_value_1);
                sin_value_2 = Math.Sqrt(1 - cos_value_2 * cos_value_2);

                double x_1 = circle_1.r * cos_value_1 + circle_1.center.x;
                double x_2 = circle_1.r * cos_value_2 + circle_1.center.x;
                double y_1 = circle_1.r * sin_value_1 + circle_1.center.y;
                double y_2 = circle_1.r * sin_value_2 + circle_1.center.y;
                point crossPoint_1 = new point(x_1, y_1);
                point crossPoint_2 = new point(x_2, y_2);

                if (!double_equals(distance(crossPoint_1, circle_2.center), circle_2.r))
                {
                    crossPoint_1.y = circle_1.center.y - circle_1.r * sin_value_1;
                }
                if (!double_equals(distance(crossPoint_2, circle_2.center), circle_2.r))
                {
                    crossPoint_2.y = circle_1.center.y - circle_1.r * sin_value_2;
                }
                if (double_equals(crossPoint_1.y, crossPoint_2.y)
                    && double_equals(crossPoint_1.x, crossPoint_2.x))
                {
                    if (crossPoint_1.y > 0)
                    {
                        crossPoint_2.y = -crossPoint_2.y;
                    }
                    else
                    {
                        crossPoint_1.y = -crossPoint_1.y;
                    }
                }
                lstPoint.Add(crossPoint_1);
                lstPoint.Add(crossPoint_2);
                return lstPoint;
            }
        }

        //判断点p0是否在弧上
        public static bool dot_onhu_in(point p0, Hu hu)
        {
            //using(Polyline pl = new Polyline())
            //{
            //    pl.AddVertexAt(0, new Point2d(hu.from.x, hu.from.y), hu.h, 0, 0);
            //    pl.AddVertexAt(1, new Point2d(hu.to.x, hu.to.y), 0, 0, 0);

                
            //}
            try
            {
                bool bInHu = false;
                if (true == distance(p0, hu.from) < 1e-6
                    || true == distance(p0, hu.to) < 1e-6)
                {
                    bInHu = true;
                }
                else
                {
                    point huTop = getHutop(hu);
                    point center_std = getCenter(hu.from, hu.to, huTop);
                    point center_tmp = getCenter(hu.from, hu.to, p0);

                    if (null != center_tmp)
                    {
                        double d = distance(center_std, center_tmp);
                        if (d < 1e-1
                            && false == opposite_side(p0, huTop, hu.from, hu.to))
                        {
                            bInHu = true;
                        }
                    }
                }
                return bInHu;
            }
            catch(Exception e)
            { throw e; }
        }

        //求两弧交点
        public static List<point> intersection_hu(Hu hu_1, Hu hu_2)
        {
            List<point> lstPoint = new List<point>();
            point center_1 = getCenter(hu_1.from, hu_1.to, getHutop(hu_1));
            double r_1 = distance(hu_1.from, center_1);
            point center_2 = getCenter(hu_2.from, hu_2.to, getHutop(hu_2));
            double r_2 = distance(hu_2.from, center_2);
            //弧所在的圆
            Circle circle_1 = new Circle(center_1, r_1);
            Circle circle_2 = new Circle(center_2, r_2);
            //圆的交点
            List<point> circle_intersection = intersection_circle(circle_1, circle_2);

            //遍历所在圆的交点
            foreach (point p in circle_intersection)
            {
                //弧顶与交点在弦的同侧（分别对于两个弧来说），则为弧的交点
                if (false == opposite_side(p, getHutop(hu_1), hu_1.from, hu_1.to)
                    && false == opposite_side(p, getHutop(hu_2), hu_2.from, hu_2.to))
                {
                    lstPoint.Add(p);
                }
            }

            return lstPoint;
        }
    }
}
