using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using PolylineGetter.Algorithm;

namespace PolylineGetter.Tools
{
    public class mytools
    {
        //数据转化
        public static MyPolyline GetMyPolylineFrom(Polyline polyline)
        {
          
            //0个点
            if (0 == polyline.NumberOfVertices)
            {
                return null;
            }

            MyPolyline retMyPolyline = new MyPolyline(null, 0);
            MyNode pre = new MyNode();
            MyNode cur = new MyNode();
            bool started = false;
            for (int ind = 0; ind < polyline.NumberOfVertices; ind++)
            {
                // polyline.GetPoint2dAt(xxx) <- 获取的坐标受Normal的影响，所以为了避免影响，获取三维点再转成二维点
                var pt = polyline.GetPoint3dAt(ind);
                Point2d p2d = new Point2d(pt.X, pt.Y);

                double bulge = -polyline.GetBulgeAt(ind);//AutoCAD中顺时针为负，开发要求顺时针为正
                cur = new MyNode(p2d.X, p2d.Y, bulge);

                //首个点
                if (false == started)
                {
                    retMyPolyline = new MyPolyline(cur, polyline.NumberOfVertices);
                    started = true;
                }
                //非首个点
                else
                {
                    //更新数据结构
                    pre.setNextNode(cur);
                    cur.setPreNode(pre);
                }
                pre = cur;
            }

            //处理最后一个点（闭合）
            if (true == polyline.Closed)
            {
                if (null != cur)
                {
                    cur.setNextNode(retMyPolyline.StartNode);
                    retMyPolyline.StartNode.setPreNode(cur);
                }
            }
            //处理最后一个点（非闭合）
            else
            {
                if (null != cur)
                {
                    cur.setNextNode(null);
                    retMyPolyline.StartNode.setPreNode(null);
                }
            }

            return retMyPolyline;
        }

        //数据转化
        public static Polyline ConvertToPolyline(MyPolyline myPolyline)
        {
            Polyline retPolyline = new Polyline();
            MyNode curNode = new MyNode(myPolyline.StartNode);
            for (int ind = 0; ind < myPolyline.NodeCount; ind++)
            {
                Point2d p2d = new Point2d(Math.Round(curNode.X, 1), Math.Round(curNode.Y, 1));//取一位小数,是出于防止cad生成polyline被判断为自交的原因，例如圆弧相交产生的交点被判断为自交（参考圆角指令）
                double bulge = -curNode.H;//AutoCAD中顺时针为负，开发要求顺时针为正
                retPolyline.AddVertexAt(ind, p2d, bulge, 0, 0);
                curNode = new MyNode(curNode.NEXT);
            }
            if (null == myPolyline.StartNode.PRE)
            {
                retPolyline.Closed = false;
            }
            else
            {
                retPolyline.Closed = true;
            }
            return retPolyline;
        }
    }
}
