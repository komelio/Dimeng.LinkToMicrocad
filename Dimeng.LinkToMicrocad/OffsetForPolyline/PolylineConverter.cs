using Autodesk.AutoCAD.DatabaseServices;
using Offset;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Offset
{
    class PolylineConverter
    {
        public MyPolyline ConverToMyPolyline(Polyline lwp)
        {
            //todo:检测多段线的类型，并不是所有的多段线都是可以转换的

            List<SEG> segments = new List<SEG>();

            int vn = lwp.NumberOfVertices;
            for (int i = 0; i < vn; i++)
            {
                // 也可以在这里获取3D点     
                segments.Add(new SEG((lwp.GetPoint2dAt(i)), (lwp.GetBulgeAt(i))));
            }
            MyPolyline myPolyline = new MyPolyline(segments);
            myPolyline.IsClosed = lwp.Closed;
            myPolyline.DealWithClosedSituation();
            myPolyline = MyPolyline.Pretreatment(myPolyline);

            return myPolyline;
        }
    }
}
