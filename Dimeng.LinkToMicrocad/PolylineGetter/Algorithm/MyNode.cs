using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//多线段数据结构上的某个点元素类（双链表结点）

namespace PolylineGetter.Algorithm
{
    //描述多线段上的一个点，链表结点
    public class MyNode
    {
        private double m_x;
        public double X
        {
            get { return m_x; }
            //set { m_x = X; }
        }
        private double m_y;
        public double Y
        {
            get { return m_y; }
            //set { m_y = Y; }
        }
        private double m_h;
        public double H
        {
            get { return m_h; }
            //set { m_h= H; }
        }
        private MyNode m_pre;
        public MyNode PRE
        {
            get { return m_pre; }
            //set { m_pre = PRE; }
        }
        private MyNode m_next;
        public MyNode NEXT
        {
            get { return m_next; }
            //set { m_next = NEXT; }
        }

        //构造函数
        public MyNode()
        {
            m_x = 0;
            m_y = 0;
            m_h = 0;
            m_pre = m_next = null;
        }

        public MyNode(double x, double y, double h)
        {
            m_x = x;
            m_y = y;
            m_h = h;
            m_pre = m_next = null;        
        }

        public MyNode(MyNode myNode)
        {
            if (null != myNode)
            {
                m_x = myNode.X;
                m_y = myNode.Y;
                m_h = myNode.H;
                m_pre = myNode.PRE;
                m_next = myNode.NEXT;
            }
        }

        public void setNextNode(MyNode myNode)
        {
            m_next = myNode;
        }

        public void setPreNode(MyNode myNode)
        {
            m_pre = myNode;
        }

        public void setH(double h)
        {
            m_h = h;
        }

        public void insertNodeAfterSelf(MyNode myNode)
        {
            MyNode temp = m_next;
            m_next = new MyNode(myNode);
            m_next.setPreNode(this);
            m_next.setNextNode(temp);
            temp.setPreNode(m_next);
        }

        public bool IsSameNode(MyNode myNode)
        {
            bool isSame = false;

            //由于存在误差,认为距离小于0.01的两个点就是一样的,这主要在于考量交点的重复性
            //例如,两段圆弧和直线相切,产生的交点可能因为误差的原因,导致出现两个交点
            double dist = Math.Sqrt(Math.Pow(m_x - myNode.X, 2) + Math.Pow(m_y - myNode.Y, 2));
            if( dist < 0.1)
            {
                isSame = true;
            }

            return isSame;
        }
    }
}
