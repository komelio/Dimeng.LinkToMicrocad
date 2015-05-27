using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//纯粹的几何计算类

namespace PolylineGetter.Algorithm
{

    //对多线段进行几何处理类
    class Geometry
    {
        //在三个点确定的原弧上，求新弧myNode_0->myNode_1的供高比
        public static double GetBulge(MyNode myNode_0, MyNode myNode_1, MyNode myNode_2)
        {
            double Flag = 0;
            if (Math.Abs(myNode_0.H) > 1e-6)
            {
                Flag = myNode_0.H / Math.Abs(myNode_0.H);
            }


            point p0 = new point(myNode_0.X, myNode_0.Y);
            point p1 = new point(myNode_1.X, myNode_1.Y);
            point p2 = new point(myNode_2.X, myNode_2.Y);

            //三点共线
            if (true == MathGeometry.parallel(p0, p1, p1, p2))
            {
                return 0.0f;
            }

            double bulge = 0;

            //完整的原弧
            Hu hu_02 = new Hu(p0, p2, myNode_0.H);

            //圆心
            point center = MathGeometry.getCenter(p0, p1, p2);
            //半径
            double r = MathGeometry.distance(p0, center);

            //新弧的弦中点
            point midXian = new point(
                (myNode_0.X + myNode_1.X) / 2,
                (myNode_0.Y + myNode_1.Y) / 2
                );

            //新弧弦中点到圆心所在的直线与原弧的交点
            List<point> huCross = MathGeometry.intersection_line_hu(center, midXian, hu_02);

            point realCross = null;
            foreach (point pt in huCross)
            {
                double d = MathGeometry.distance(pt, midXian);
                if (Math.Abs(hu_02.h) - 1.0f > 1e-6)//原弧大于半圆
                {
                    //p1, p2在直线p0->center的异侧
                    if (true == MathGeometry.opposite_side(p1, p2, p0, center))
                    {
                        //新弧小于等于半圆
                        if (d <= r)
                        {
                            realCross = pt;
                            break;
                        }
                    }
                    //p1, p2在直线p0->center的同侧
                    else
                    {
                        //新弧大于半圆
                        if (d > r)
                        {
                            realCross = pt;
                            break;
                        }
                    }
                }
                else//原弧小于半圆
                {
                    //那么新弧也必定小于半圆
                    if (d < r)
                    {
                        realCross = pt;
                        break;
                    }
                }
            }

            //更新供高比
            if (null != realCross)
            {
                double gonggao = MathGeometry.distance(midXian, realCross);
                bulge = 2 * gonggao / MathGeometry.distance(p0, p1);
            }

            bulge *= Flag;

            return bulge;
        }

        //在三个点确定的原弧上，求新弧myNode_1->myNode_2的供高比
        public static double GetBulgeEx(MyNode myNode_0, MyNode myNode_1, MyNode myNode_2)
        {
            double Flag = 0;
            if (Math.Abs(myNode_0.H) > 1e-6)
            {
                Flag = myNode_0.H / Math.Abs(myNode_0.H);
            }


            point p0 = new point(myNode_0.X, myNode_0.Y);
            point p1 = new point(myNode_1.X, myNode_1.Y);
            point p2 = new point(myNode_2.X, myNode_2.Y);

            //三点共线
            if (true == MathGeometry.parallel(p0, p1, p1, p2))
            {
                return 0.0f;
            }

            double bulge = 0;
            //完整的原弧
            Hu hu_02 = new Hu(p0, p2, myNode_0.H);

            //圆心
            point center = MathGeometry.getCenter(p0, p1, p2);
            //半径
            double r = MathGeometry.distance(p0, center);

            //新弧的弦中点
            point midXian = new point(
                (myNode_1.X + myNode_2.X) / 2,
                (myNode_1.Y + myNode_2.Y) / 2
                );

            //新弧弦中点到圆心所在的直线与原弧的交点
            List<point> huCross = MathGeometry.intersection_line_hu(center, midXian, hu_02);

            point realCross = null;
            foreach (point pt in huCross)
            {
                double d = MathGeometry.distance(pt, midXian);
                if (Math.Abs(hu_02.h) - 1.0f > 1e-6)//原弧大于半圆
                {
                    //p0, p1在直线p2->center的异侧
                    if (true == MathGeometry.opposite_side(p0, p1, p2, center))
                    {
                        //新弧小于等于半圆
                        if (d <= r)
                        {
                            realCross = pt;
                            break;
                        }
                    }
                    //p0, p1在直线p2->center的同侧
                    else
                    {
                        //新弧大于半圆
                        if (d > r)
                        {
                            realCross = pt;
                            break;
                        }
                    }
                }
                else//原弧小于半圆
                {
                    //那么新弧也必定小于半圆
                    if (d < r)
                    {
                        realCross = pt;
                        break;
                    }
                }
            }

            //更新供高比
            if (null != realCross)
            {
                double gonggao = MathGeometry.distance(midXian, realCross);
                bulge = 2 * gonggao / MathGeometry.distance(p1, p2);
            }

            bulge *= Flag;

            return bulge;
        }

        //判断点myNode_0是否在元素myNode_1->myNode_2上
        public static bool IsInElement(MyNode myNode_0, MyNode myNode_1, MyNode myNode_2)
        {
            try
            {
                bool bIsInElement = false;
                point p0 = new point(myNode_0.X, myNode_0.Y);
                point p1 = new point(myNode_1.X, myNode_1.Y);
                point p2 = new point(myNode_2.X, myNode_2.Y);
                //线段
                if (Math.Abs(myNode_1.H) < 1e-6)
                {
                    //点p0在线段p1-p2上
                    if (MathGeometry.dot_online_in(p0, p1, p2))
                    {
                        bIsInElement = true;
                    }
                }
                //弧
                else
                {
                    double h = myNode_1.H;
                    //点p0在弧p1-p2上
                    if (MathGeometry.dot_onhu_in(p0, new Hu(p1, p2, h)))
                    {
                        bIsInElement = true;
                    }
                }

                return bIsInElement;
            }
            catch
            {
                throw;
            }
        }

        //求两段多线段元素的交点node_1_start->node_1_end,node_2_start->node_2_end
        private static List<MyNode> GetLineIntersectNodes(
            MyNode node_1_start,
            MyNode node_1_end,
            MyNode node_2_start,
            MyNode node_2_end)
        {
            List<MyNode> lstMyNode = new List<MyNode>();

            //线段与线段(供高比都小于1e-6)
            if (Math.Abs(node_1_start.H) < 1e-6 && Math.Abs(node_2_start.H) < 1e-6)
            {
                //线段u1->u2
                point u1 = new point(node_1_start.X, node_1_start.Y);
                point u2 = new point(node_1_end.X, node_1_end.Y);
                //线段v1->v2
                point v1 = new point(node_2_start.X, node_2_start.Y);
                point v2 = new point(node_2_end.X, node_2_end.Y);

                point result = MathGeometry.intersection_segment(u1, u2, v1, v2);
                if (null != result)
                {
                    MyNode myResult = new MyNode(result.x, result.y, 0);
                    lstMyNode.Add(myResult);
                }
            }

            //线段与圆弧（有一个供高比小于1e-6）
            else if (Math.Abs(node_1_start.H) < 1e-6 || Math.Abs(node_2_start.H) < 1e-6)
            {
                //线段u1->u2
                point u1 = null;
                point u2 = null;
                //弧
                Hu hu = null;
                //1为直线2为弧
                if (Math.Abs(node_1_start.H) < 1e-6)
                {
                    u1 = new point(node_1_start.X, node_1_start.Y);
                    u2 = new point(node_1_end.X, node_1_end.Y);

                    point v1 = new point(node_2_start.X, node_2_start.Y);
                    point v2 = new point(node_2_end.X, node_2_end.Y);
                    hu = new Hu(v1, v2, node_2_start.H);
                }
                //2为直线1为弧
                else if (Math.Abs(node_2_start.H) < 1e-6)
                {
                    u1 = new point(node_2_start.X, node_2_start.Y);
                    u2 = new point(node_2_end.X, node_2_end.Y);

                    point v1 = new point(node_1_start.X, node_1_start.Y);
                    point v2 = new point(node_1_end.X, node_1_end.Y);
                    hu = new Hu(v1, v2, node_1_start.H);
                }
                //线段与弧非空
                if (null != u1 && null != u2 && null != hu)
                {
                    List<point> lstPoint = MathGeometry.intersection_segment_hu(u1, u2, hu);
                    foreach (point result in lstPoint)
                    {
                        if (null != result)
                        {
                            MyNode myResult = new MyNode(result.x, result.y, 0);
                            lstMyNode.Add(myResult);
                        }
                    }
                }
            }

            //圆弧与圆弧
            else
            {
                point u1 = new point(node_1_start.X, node_1_start.Y);
                point u2 = new point(node_1_end.X, node_1_end.Y);
                Hu hu1 = new Hu(u1, u2, node_1_start.H);
                point v1 = new point(node_2_start.X, node_2_start.Y);
                point v2 = new point(node_2_end.X, node_2_end.Y);
                Hu hu2 = new Hu(v1, v2, node_2_start.H);

                //两弧交点
                List<point> lstPoint = MathGeometry.intersection_hu(hu1, hu2);
                foreach (point result in lstPoint)
                {
                    if (null != result)
                    {
                        MyNode myResult = new MyNode(result.x, result.y, 0);
                        lstMyNode.Add(myResult);
                    }
                }
            }

            return lstMyNode;
        }

        //求两条MyPolyline的交点
        public static List<MyNode> GetIntersectsNodes(
            MyPolyline myPolyline_1,
            MyPolyline myPolyline_2)
        {
            List<MyNode> lstMyNode = new List<MyNode>();
            //多线段1的当前点
            MyNode cur_1 = null;
            if (null != myPolyline_1.StartNode)
            {
                cur_1 = new MyNode(myPolyline_1.StartNode);
            }

            for (int ind_i = 0; ind_i < myPolyline_1.NodeCount; ind_i++)
            {
                //多线段1的下一点
                MyNode new_cur_1 = null;
                if (null != cur_1.NEXT)
                {
                    new_cur_1 = new MyNode(cur_1.NEXT);
                }

                //多线段2的当前点
                MyNode cur_2 = null;
                if (null != myPolyline_2.StartNode)
                {
                    cur_2 = new MyNode(myPolyline_2.StartNode);
                }
                for (int ind_j = 0; ind_j < myPolyline_2.NodeCount; ind_j++)
                {

                    //多线段2的下一点
                    MyNode new_cur_2 = null;
                    if (null != cur_2.NEXT)
                    {
                        new_cur_2 = new MyNode(cur_2.NEXT);
                    }

                    //有点不存在
                    if (null == cur_1
                        || null == cur_2
                        || null == new_cur_1
                        || null == new_cur_2)
                    {
                        continue;
                    }

                    //当前段之间的交点
                    List<MyNode> currentIntersects = new List<MyNode>();
                    currentIntersects = GetLineIntersectNodes(cur_1, new_cur_1, cur_2, new_cur_2);

                    //将当前段交点加入交点集合
                    foreach (MyNode myNode in currentIntersects)
                    {
                        bool sameNode = false;
                        foreach (MyNode existNode in lstMyNode)
                        {
                            if (true == myNode.IsSameNode(existNode))
                            {
                                sameNode = true;
                                break;
                            }
                        }
                        if (false == sameNode)
                        {
                            lstMyNode.Add(myNode);
                        }
                    }

                    //更新多线段1的当前点

                    if (null != new_cur_2)
                    {
                        cur_2 = new MyNode(new_cur_2);
                    }
                    else
                    {
                        cur_2 = null;
                        break;
                    }
                }
                //更新多线段1的当前点
                if (null != new_cur_1)
                {
                    cur_1 = new MyNode(new_cur_1);
                }
                else
                {
                    cur_1 = null;
                    break;
                }
            }

            //test:增加一个点的过滤,将相邻过于近的点拼成一个点,减少点的数量
            var tempNodes = new List<MyNode>();
            foreach (var node in lstMyNode)
            {
                bool isExisted = false;
                foreach (var tNode in tempNodes)
                {
                    if(tNode.IsSameNode(node))
                    {
                        isExisted = true;
                        break;
                    }
                }

                if (!isExisted)
                {
                    tempNodes.Add(node);
                }
            }

            return tempNodes;
        }

        //求多线段元素node_start->node_end的中点
        public static MyNode GetMid(MyNode node_start, MyNode node_end)
        {
            if (null == node_start
                || null == node_end)
            {
                return null;
            }
            else if (Math.Abs(node_start.H) < 1e-6)
            {
                return (new MyNode(
                    (node_start.X + node_end.X) / 2,
                    (node_start.Y + node_end.Y) / 2,
                    0)
                    );
            }
            else
            {
                point p0 = new point(node_start.X, node_start.Y);
                point p1 = new point(node_end.X, node_end.Y);
                Hu hu = new Hu(p0, p1, node_start.H);
                point pt = MathGeometry.getHutop(hu);
                return (new MyNode(pt.x, pt.y, 0));
            }
        }

        //判断点是否在封闭多线段myPolyline内
        public static bool IsNodeInMyPolyline(MyPolyline myPolyline, MyNode pos)
        {
            int randCheckTime = 10;//随机算法执行次数，建议>=3，反正对效率无要求
            int passTime = 0;
            for (int count = 1; count <= randCheckTime; count++)
            {
                //由于随机算法可能碰到特殊情况导致检测错误，所以多算几遍
                if (true == randCheckIsNodeInMyPolyline(myPolyline, pos))
                {
                    passTime++;
                }
            }
            bool bIsIn = false;
            if (passTime > randCheckTime / 2)
            {
                bIsIn = true;
            }
            return bIsIn;
        }

        //判断点是否在封闭多线段myPolyline内(一次随机算法)
        private static bool randCheckIsNodeInMyPolyline(MyPolyline myPolyline, MyNode pos)
        {
            List<point> lstPoint = new List<point>();

            //构造一条过点pos的直线
            Random r = new Random();
            point p0 = new point(pos.X, pos.Y);
            point p1 = new point(p0.x + r.Next(100) + 10, p0.y + r.Next(100) + 10);
            if (p0.x == p1.x)
            {
                p1.x += r.Next(10);
            }
            if (p0.y == p1.y)
            {
                p1.y += r.Next(10);
            }

            //多线段当前点
            MyNode cur = null;
            if (null != myPolyline.StartNode)
            {
                cur = new MyNode(myPolyline.StartNode);
            }

            for (int ind_i = 0; ind_i < myPolyline.NodeCount; ind_i++)
            {
                //多线段下一点
                MyNode new_cur = null;
                if (null != cur.NEXT)
                {
                    new_cur = new MyNode(cur.NEXT);
                }

                //当前段
                if (null != cur && null != new_cur)
                {
                    point p2 = new point(cur.X, cur.Y);
                    point p3 = new point(new_cur.X, new_cur.Y);
                    //线段
                    if (Math.Abs(cur.H) < 1e-6)
                    {
                        if (false == MathGeometry.parallel(p0, p1, p2, p3))
                        {
                            point cross = MathGeometry.intersection_line(p0, p1, p2, p3);
                            if (null != cross
                                && MathGeometry.dot_online_in(cross, p2, p3))
                            {
                                bool exist = false;
                                foreach (point pt in lstPoint)
                                {
                                    if (true == pt.IsSamePoint(cross))
                                    {
                                        exist = true;
                                        break;
                                    }
                                }
                                if (false == exist)
                                {
                                    lstPoint.Add(cross);
                                }
                            }
                        }
                    }
                    //弧
                    else
                    {
                        Hu hu = new Hu(p2, p3, cur.H);
                        List<point> lstCross = MathGeometry.intersection_line_hu(p0, p1, hu);
                        foreach (point cross in lstCross)
                        {
                            bool exist = false;
                            foreach (point pt in lstPoint)
                            {
                                if (true == pt.IsSamePoint(cross))
                                {
                                    exist = true;
                                    break;
                                }
                            }
                            if (false == exist)
                            {
                                lstPoint.Add(cross);
                            }
                        }
                    }
                }

                //更新多线段当前点
                if (null != new_cur)
                {
                    cur = new MyNode(new_cur);
                }
            }

            bool bIsIn = false;
            if (0 < lstPoint.Count)
            {
                int leftCount = 0;
                foreach (point pt in lstPoint)
                {
                    if (pt.x < p0.x)
                    {
                        leftCount++;
                    }
                }
                if (1 == (leftCount % 2))
                {
                    bIsIn = true;
                }
            }

            if (false == bIsIn)
            {
                return false;
            }

            return bIsIn;
        }

        //判断点mynode0是否是在点mynode1至mynode2连线的左侧
        public static bool CheckLeft(MyNode mynode0, MyNode mynode1, MyNode mynode2)
        {
            bool isLeft = false;

            point p0 = new point(mynode0.X, mynode0.Y);
            point p1 = new point(mynode1.X, mynode1.Y);
            point p2 = new point(mynode2.X, mynode2.Y);
            double f = (p2.x - p1.x) * (p0.y - p1.y) - (p0.x - p1.x) * (p2.y - p1.y);
            if (f > 1e-6)
            {
                isLeft = true;
            }
            return isLeft;
        }

        //判断点mynode0是否是在点mynode1至mynode2连线的右侧
        public static bool CheckRight(MyNode mynode0, MyNode mynode1, MyNode mynode2)
        {
            bool isRight = false;

            point p0 = new point(mynode0.X, mynode0.Y);
            point p1 = new point(mynode1.X, mynode1.Y);
            point p2 = new point(mynode2.X, mynode2.Y);
            double f = (p2.x - p1.x) * (p0.y - p1.y) - (p0.x - p1.x) * (p2.y - p1.y);
            if (f < -1e-6)
            {
                isRight = true;
            }
            return isRight;
        }
    }
}
