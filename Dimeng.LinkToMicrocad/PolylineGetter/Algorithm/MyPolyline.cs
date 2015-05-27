using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//多线段数据结构类

namespace PolylineGetter.Algorithm
{
    public class MyPolyline
    {
        //链表表头 
        private MyNode m_startNode;
        public MyNode StartNode
        {
            get { return m_startNode; }
            set { m_startNode = StartNode; }
        }
        
        //结点数量
        private int m_nodeCount;
        public int NodeCount
        {
            get { return m_nodeCount; }
            //set { m_nodeCount = NodeCount; }
        }

        public MyPolyline()
        {
            m_startNode = new MyNode();
            m_nodeCount = 0;
        }

        public MyPolyline(MyPolyline myPolyline)
        {
            m_startNode = new MyNode(myPolyline.StartNode);
            MyNode pre = m_startNode;
            MyNode cur = null;
            MyNode cur_temp = myPolyline.StartNode;
            for (int ind = 1; ind < myPolyline.NodeCount; ind++)
            {
                cur_temp = cur_temp.NEXT;
                cur = new MyNode(cur_temp);
                cur.setPreNode(pre);
                pre.setNextNode(cur);

                pre = cur;
            }
            if (true == myPolyline.IsClose())
            {
                if (null != cur)
                {
                    cur.setNextNode(m_startNode);
                    m_startNode.setPreNode(cur);
                }
            }
            else
            {
                if (null != cur)
                {
                    cur.setNextNode(null);
                    m_startNode.setPreNode(null);
                }
            }
            m_nodeCount = myPolyline.NodeCount;
        }

        public MyPolyline(MyNode myNode, int num)
        {
            m_startNode = myNode;
            m_nodeCount = num;
        }

        //让点数计数+1
        public void AddCount()
        {
            m_nodeCount++;
        }

        //是否闭合
        public bool IsClose()
        {
            bool retIsClose = true;
            if (null == m_startNode.PRE)
            {
                retIsClose = false;
            }
            return retIsClose;
        }
    }
}
