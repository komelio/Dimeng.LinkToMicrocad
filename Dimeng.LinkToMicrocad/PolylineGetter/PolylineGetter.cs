using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using PolylineGetter.Algorithm;
using PolylineGetter.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolylineGetter
{
    public class PolylineGetter
    {
        public List<Polyline> CalculatePolyline(Polyline mainPolyline, Polyline routeLine, bool isLeftComp)
        {
            Point3dCollection points1 = new Point3dCollection();
            mainPolyline.IntersectWith(routeLine, Intersect.OnBothOperands, points1, IntPtr.Zero, IntPtr.Zero);
            List<MyNode> intersectsNodes = new List<MyNode>();
            foreach (Point3d pt in points1)
            {
                intersectsNodes.Add(new MyNode(pt.X, pt.Y, 0));
            }

            MyPolyline objMyPolyline = mytools.GetMyPolylineFrom(mainPolyline);
            MyPolyline knifeMyPolyline = mytools.GetMyPolylineFrom(routeLine);

            List<MyPolyline> result = Divider.getDividResult(objMyPolyline, knifeMyPolyline, intersectsNodes, !isLeftComp, isLeftComp);

            List<Polyline> lines = new List<Polyline>();
            foreach (MyPolyline mp in result)
            {
                lines.Add(Tools.mytools.ConvertToPolyline(mp));
            }

            return lines;
        }
    }
}
