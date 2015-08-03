using Dimeng.LinkToMicrocad.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//多线段分割计算类

namespace PolylineGetter.Algorithm
{
    public class Divider
    {
        //判断 preKnifeNode and curKnifeNode 是否在 myPolyline 上
        private static bool IsCutted(
            MyPolyline myPolyline,
            MyNode preKnifeNode,
            MyNode curKnifeNode,
            out MyNode objPreNode,
            out MyNode objCurNode)
        {
            objPreNode = null;
            objCurNode = null;


            bool findPre = false;
            bool findCur = false;
            MyNode curNode = new MyNode(myPolyline.StartNode);
            for (int ind = 0; ind < myPolyline.NodeCount; ind++)
            {
                if (true == curNode.IsSameNode(curKnifeNode))
                {
                    findCur = true;
                    objCurNode = new MyNode(curNode);
                }
                if (true == curNode.IsSameNode(preKnifeNode))
                {
                    findPre = true;
                    objPreNode = new MyNode(curNode);
                }
                curNode = new MyNode(curNode.NEXT);
            }

            bool retbIsCutted = false;
            if (true == findPre && true == findCur)
            {
                retbIsCutted = true;
            }
            return retbIsCutted;
        }

        //多线段划分算法
        private static List<MyPolyline> getMyDividResult(
            MyPolyline objMyPolyline,
            MyPolyline knifeMyPolyline,
            List<bool> lstObjIsIntersec,
            List<bool> lstKnifeIsIntersec,
            bool left,
            bool right)
        {
            //保存当前值和上一次分割值
            List<MyPolyline> prelstMyPolyLine = new List<MyPolyline>();
            List<MyPolyline> curlstMyPolyLine = new List<MyPolyline>();
            curlstMyPolyLine.Add(objMyPolyline);

            MyNode currentKnife = new MyNode(knifeMyPolyline.StartNode);
            int knifeLength = knifeMyPolyline.NodeCount;
            MyNode curIntersec = null;
            MyNode preIntersec = null;
            MyNode firstIntersec = null;
            MyNode lastIntersec = null;

            //遍历多线段B上的点，找到交点就启动分割
            for (int ind = 0; ind < knifeLength; ind++)
            {
                //如果是闭环而且是最后一个点了，判断是否要进行判断第一个交点和最后一个交点形成的切割
                if (ind == knifeLength - 1 && knifeMyPolyline.IsClose() && firstIntersec != null)
                {
                    Logger.GetLogger().Debug("Last index on loop");
                    preIntersec = lastIntersec;
                    curIntersec = firstIntersec;

                    Logger.GetLogger().Debug(string.Format("PreIntersec:{0}/{1}", preIntersec.X, preIntersec.Y));
                    Logger.GetLogger().Debug(string.Format("CurIntersec:{0}/{1}", curIntersec.X, curIntersec.Y));

                    prelstMyPolyLine = cutPolyline(left, right, prelstMyPolyLine, curlstMyPolyLine, curIntersec, preIntersec);
                    break;
                }
                //如果相交
                if (lstKnifeIsIntersec[ind])
                {
                    curIntersec = new MyNode(currentKnife);

                    //记录第一个交点
                    if (firstIntersec == null)
                    {
                        firstIntersec = curIntersec;
                        lastIntersec = firstIntersec;
                    }

                    //记录最后一个交点
                    if (lastIntersec != null)
                    {
                        lastIntersec = curIntersec;
                    }

                    //发现一对交点
                    if (preIntersec != null)
                    {
                        prelstMyPolyLine = cutPolyline(left, right, prelstMyPolyLine, curlstMyPolyLine, curIntersec, preIntersec);
                    }
                    preIntersec = new MyNode(currentKnife);
                }
                currentKnife = new MyNode(currentKnife.NEXT);
            }
            return curlstMyPolyLine;
        }

        private static List<MyPolyline> cutPolyline(bool left, bool right, List<MyPolyline> prelstMyPolyLine, List<MyPolyline> curlstMyPolyLine, MyNode curIntersec, MyNode preIntersec)
        {
            //更新数据
            prelstMyPolyLine = new List<MyPolyline>(curlstMyPolyLine);
            curlstMyPolyLine.Clear();

            #region 一次切割算法
            foreach (MyPolyline myPolyline in prelstMyPolyLine)
            {
                //preIntersec and curIntersec 是 knifePline 上的两个点
                //objPre和objCur是objPline上的两个点
                MyNode objPre = new MyNode();
                MyNode objCur = new MyNode();

                //该多线段上有这两个交点
                if (true == IsCutted(myPolyline, preIntersec, curIntersec, out objPre, out objCur))
                {
                    MyNode pos1 = Geometry.GetMid(preIntersec, preIntersec.NEXT);
                    MyNode pos2 = Geometry.GetMid(curIntersec.PRE, curIntersec);

                    //判断两个交点是否相邻,必须同时是knife上和mypline上的相邻点
                    bool isBeside = false;
                    if (curIntersec.PRE != null)
                    {
                        if (curIntersec.PRE.IsSameNode(preIntersec))
                        {
                            isBeside = true;
                        }
                    }
                    else if (curIntersec.NEXT != null)
                    {
                        if (curIntersec.NEXT.IsSameNode(preIntersec))
                        {
                            isBeside = true;
                        }
                    }
                    isBeside = isBeside && (objCur.PRE.IsSameNode(objPre) || objCur.NEXT.IsSameNode(objPre));

                    //是否确实为分割线
                    bool isRealKnife =
                        Geometry.IsNodeInMyPolyline(myPolyline, pos1)
                        && Geometry.IsNodeInMyPolyline(myPolyline, pos2);

                    if (true == isRealKnife && !isBeside)
                    {
                        //方向计算参考点
                        MyNode node_a = new MyNode(curIntersec.PRE);//切割线最后一个方向起点
                        MyNode node_b = new MyNode(curIntersec);//切割线最后一个方向终点
                        MyNode node_c = Geometry.GetMid(objCur, objCur.NEXT);//mypolyline1的参考点
                        MyNode node_d = Geometry.GetMid(objCur.PRE, objCur);//mypolyline2的参考点
                        bool is1Left = false;//mypolyline1是否是左边的
                        if (true == Geometry.CheckLeft(node_c, node_a, node_b)//c在ab左
                            && true == Geometry.CheckRight(node_d, node_a, node_b))//d在ab右
                        {
                            is1Left = true;
                        }
                        //以下，cd在ab同侧，且c在bd左
                        else if (false == Geometry.CheckLeft(node_c, node_a, node_b)//c不在ab左
                            && false == Geometry.CheckLeft(node_d, node_a, node_b)//d不在ab左
                            && true == Geometry.CheckLeft(node_c, node_b, node_d))//c在bd左
                        {
                            is1Left = true;
                        }
                        else if (false == Geometry.CheckRight(node_c, node_a, node_b)//c不在ab右
                            && false == Geometry.CheckRight(node_d, node_a, node_b)//d不在ab右
                            && true == Geometry.CheckLeft(node_c, node_b, node_d))//c在bd左
                        {
                            is1Left = true;
                        }


                        #region 计算分割后的多线段_1
                        MyPolyline newMyPolyline_1 = new MyPolyline(null, 0);
                        MyNode pre = new MyNode();
                        pre = null;
                        MyNode cur = new MyNode(objCur);
                        while (false == cur.IsSameNode(objPre))//加入边界多段线上的点
                        {
                            //首个点
                            if (null == pre)
                            {
                                newMyPolyline_1 = new MyPolyline(cur, 1);
                            }
                            //非首个点
                            else
                            {
                                //更新数据结构
                                pre.setNextNode(cur);
                                cur.setPreNode(pre);
                                newMyPolyline_1.AddCount();
                            }
                            pre = cur;
                            cur = new MyNode(cur.NEXT);
                        }
                        cur = new MyNode(preIntersec);
                        while (false == cur.IsSameNode(curIntersec))//加入切割多段线上的点
                        {
                            //更新数据结构
                            pre.setNextNode(cur);
                            cur.setPreNode(pre);
                            newMyPolyline_1.AddCount();

                            pre = cur;
                            cur = new MyNode(cur.NEXT);
                        }
                        pre.setNextNode(newMyPolyline_1.StartNode);
                        newMyPolyline_1.StartNode.setPreNode(pre);
                        #endregion
                        #region 计算分割后的多线段_2
                        MyPolyline newMyPolyline_2 = new MyPolyline(null, 0);
                        pre = new MyNode();
                        pre = null;
                        cur = new MyNode(objPre);
                        while (false == cur.IsSameNode(objCur))
                        {
                            //首个点
                            if (null == pre)
                            {
                                newMyPolyline_2 = new MyPolyline(cur, 1);
                            }
                            //非首个点
                            else
                            {
                                //更新数据结构
                                pre.setNextNode(cur);
                                cur.setPreNode(pre);
                                newMyPolyline_2.AddCount();
                            }
                            pre = cur;
                            cur = new MyNode(cur.NEXT);
                        }
                        cur = new MyNode(curIntersec);

                        while (false == cur.IsSameNode(preIntersec))
                        {
                            //更新数据结构
                            MyNode tempCurPre = new MyNode(cur.PRE);

                            cur.setH(cur.PRE.H * -1);
                            pre.setNextNode(cur);
                            cur.setPreNode(pre);
                            newMyPolyline_2.AddCount();

                            pre = cur;
                            cur = new MyNode(tempCurPre);
                        }
                        pre.setNextNode(newMyPolyline_2.StartNode);
                        newMyPolyline_2.StartNode.setPreNode(pre);
                        #endregion

                        //加入新的多线段
                        if ((true == left && true == is1Left)
                            || (true == right && false == is1Left))
                        {
                            curlstMyPolyLine.Add(newMyPolyline_1);
                        }
                        if ((true == right && true == is1Left)
                            || (true == left && false == is1Left))
                        {
                            curlstMyPolyLine.Add(newMyPolyline_2);
                        }
                    }
                    else
                    {
                        //没有分割，还原
                        curlstMyPolyLine.Add(myPolyline);
                    }
                }
                //无交点
                else
                {
                    //没有分割，还原
                    curlstMyPolyLine.Add(myPolyline);
                }
            }
            #endregion
            return prelstMyPolyLine;
        }

        //点集合 points 插入到 mypolyline上，并更新 mypolyline
        private static MyPolyline AddNodeInPoints(
            MyPolyline myPolyline,
            List<MyNode> points,
            out List<bool> lstIsIntersec)
        {
            //最终输出的多段线
            MyPolyline retMyPolyline = new MyPolyline(myPolyline);

            #region 确定已知多段线上哪些交点是属于交点
            //算法:遍历所有多段线的点,与交点集进行比较,如果在交点集里,即认为这是一个交点
            lstIsIntersec = new List<bool>();
            MyNode temp = retMyPolyline.StartNode;
            for (int ind = 0; ind < retMyPolyline.NodeCount; ind++)
            {
                bool isIntersec = false;
                foreach (MyNode myNode in points)
                {
                    if (true == temp.IsSameNode(myNode))
                    {
                        isIntersec = true;
                        break;
                    }
                }
                lstIsIntersec.Add(isIntersec);
                temp = new MyNode(temp.NEXT);
            }
            #endregion

            MyNode node_new = null;
            if (points == null)
                return retMyPolyline;

            //遍历每个交点
            foreach (MyNode myNode in points)
            {

                //为每个交点寻找位置插入
                int pos = -1;
                MyNode real_cur = null;
                MyNode real_next = null;

                //遍历起点
                MyNode node_cur = null;
                if (null != retMyPolyline.StartNode)
                {
                    node_cur = retMyPolyline.StartNode;
                }
                for (int ind = 0; ind < retMyPolyline.NodeCount; ind++)
                {
                    //下一个点
                    MyNode node_next = null;
                    if (null != node_cur.NEXT)
                    {
                        node_next = node_cur.NEXT;
                    }

                    //均非空
                    if (null != node_cur && null != node_next)
                    {
                        //判断点myNode是否在元素段node_cur->node_next上
                        if (Geometry.IsInElement(myNode, node_cur, node_next))
                        {
                            //非边界
                            if (false == node_cur.IsSameNode(myNode)
                                && false == node_next.IsSameNode(myNode))
                            {
                                pos = ind + 1;
                                real_cur = node_cur;
                                real_next = node_next;
                            }
                        }
                    }
                    node_cur = null;
                    if (null != node_next)
                    {
                        node_cur = node_next;
                    }
                }

                //如果找到了插入位置
                if (null != real_cur && null != real_next)
                {
                    //直线
                    if (Math.Abs(real_cur.H) < 1e-6)
                    {
                        node_new = new MyNode(myNode);

                        node_new.setH(0);

                        real_cur.insertNodeAfterSelf(node_new);
                    }
                    //弧
                    else
                    {
                        node_new = new MyNode(myNode);

                        double newh = Geometry.GetBulge(real_cur, node_new, real_next);
                        double newh_2 = Geometry.GetBulgeEx(real_cur, node_new, real_next);
                        real_cur.setH(newh);
                        node_new.setH(newh_2);

                        real_cur.insertNodeAfterSelf(node_new);
                    }
                    retMyPolyline.AddCount();
                    lstIsIntersec.Insert(pos, true);
                }
            }


            return retMyPolyline;
        }

        //切割算法
        public static List<MyPolyline> getDividResult(
            MyPolyline objMyPolyline,
            MyPolyline knifeMyPolyline,
            List<MyNode> lstMyNodeIntersects,
            bool left,
            bool right)
        {
            //插入交点后，对应序号点是否为交点
            List<bool> lstObjIsIntersec = new List<bool>();
            List<bool> lstKnifeIsIntersec = new List<bool>();

            //交点
            //List<MyNode> lstMyNodeIntersects = Geometry.GetIntersectsNodes(objMyPolyline, knifeMyPolyline);

            //插入交点
            MyPolyline newObjMyPolyline = new MyPolyline();
            MyPolyline newKnifeMyPolyline = new MyPolyline();
            newObjMyPolyline = AddNodeInPoints(objMyPolyline, lstMyNodeIntersects, out lstObjIsIntersec);
            newKnifeMyPolyline = AddNodeInPoints(knifeMyPolyline, lstMyNodeIntersects, out lstKnifeIsIntersec);

            //在MyPolyline上执行切割算法
            List<MyPolyline> myDividResult
                = getMyDividResult(
                    newObjMyPolyline,
                    newKnifeMyPolyline,
                    lstObjIsIntersec,
                    lstKnifeIsIntersec,
                    left,
                    right);

            return myDividResult;
        }
    }
}
