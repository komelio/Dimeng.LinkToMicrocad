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
using PolylineGetter.Tools;

[assembly: CommandClass(typeof(PolylineGetter.MyCommands))]

namespace PolylineGetter
{
    public class MyCommands
    {

        /// <summary>
        /// 
        /// </summary>
        [CommandMethod("tplines", CommandFlags.Modal)]
        public void MyCommand() // This method can have any name
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;

            using (Polyline pl1 = CADHelper.GetPolyline("选择多段线A\n"))
            using (Polyline pl2 = CADHelper.GetPolyline("选择多段线B\n"))
            {
                if (pl1 != null && pl2 != null)
                {
                    Point3dCollection points1 = new Point3dCollection();
                    pl1.IntersectWith(pl2, Intersect.OnBothOperands, points1, IntPtr.Zero, IntPtr.Zero);

                    ed.WriteMessage("A和B的坐标：\n");
                    CADHelper.PrintAllPolylineData(pl1);
                    CADHelper.PrintAllPolylineData(pl2);

                    ed.WriteMessage("A和B用AutoCAD求出的交点：\n");
                    List<MyNode> intersectsNodes = new List<MyNode>();
                    foreach (Point3d pt in points1)
                    {
                        intersectsNodes.Add(new MyNode(pt.X, pt.Y, 0));
                        ed.WriteMessage(pt.ToString() + "\n");
                    }

                    //ed.WriteMessage("A和B不用库函数求出的交点：\n");
                    //MyPolyline myPolyline_1 = mytools.GetMyPolylineFrom(pl1);
                    //MyPolyline myPolyline_2 = mytools.GetMyPolylineFrom(pl2);
                    //List<MyNode> lstMyNode = Geometry.GetIntersectsNodes(myPolyline_1, myPolyline_2);
                    //foreach (MyNode myNode in lstMyNode)
                    //{
                    //    ed.WriteMessage("(" + myNode.X.ToString() + ", " + myNode.Y.ToString() + ")\n");
                    //}

                    //将多线段数据类型转化为MyPolyline
                    MyPolyline objMyPolyline = mytools.GetMyPolylineFrom(pl1);
                    MyPolyline knifeMyPolyline = mytools.GetMyPolylineFrom(pl2);

                    ed.WriteMessage("B将A分割为以下多线段：\n");
                    List<MyPolyline> result = Divider.getDividResult(objMyPolyline, knifeMyPolyline, intersectsNodes, false, true);
                    //List<MyPolyline> result = Divider.getDividResult(objMyPolyline, knifeMyPolyline, intersectsNodes, true, false);
                    foreach (MyPolyline mp in result)
                    {
                        CADHelper.PrintMyPolyline(mp, 0);
                    }
                }
            }
        }
    }
}
