using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Geometry;

namespace Dimeng.WoodEngine.Entities.Machinings
{
    public class HDrilling
    {
        public HDrilling(int facenumber, double diameter, double depth, double position, double zvalue, Part p)
        {
            FaceNumber = facenumber;
            Diameter = diameter;
            Depth = depth;
            Position = position;
            ZValue = zvalue;
            Part = p;
        }

        public int FaceNumber { get; private set; }
        public double Diameter { get; private set; }
        public double Depth { get; private set; }
        public double Position { get; private set; }
        public double ZValue { get; private set; }
        public Part Part { get; private set; }

        /// <summary>
        /// 根据板件是否旋转以及所在边获取孔的方位向量，用于绘制Hdrill时使用
        /// </summary>
        /// <returns>X方向或Y方向</returns>
        public Vector3d GetBoreDirection()
        {
            Vector3d holeDirection = new Vector3d();
            if (Part.MachinePoint.IsRotated)
            {
                if (FaceNumber == 3 || FaceNumber == 4)
                    holeDirection = Vector3d.XAxis;
                else holeDirection = Vector3d.YAxis;
            }
            else
            {
                if (FaceNumber == 1 || FaceNumber == 2)
                    holeDirection = Vector3d.XAxis;
                else holeDirection = Vector3d.YAxis;
            }
            return holeDirection;
        }

        /// <summary>
        /// 根据板件的尺寸和水平孔的位置，获取水平孔以MP为原点时的坐标，如果在边沿就等于0或者板件长宽
        /// </summary>
        /// <returns></returns>
        public Point3d GetBorePosition()
        {
            Point3d holePosition = new Point3d();
            if (!this.Part.MachinePoint.IsRotated)
            {
                if (FaceNumber == 1)
                    holePosition = new Point3d(Position, 0, ZValue);
                else if (FaceNumber == 2)
                    holePosition = new Point3d(Position, Part.Width, ZValue);
                else if (FaceNumber == 3)
                    holePosition = new Point3d(0, Position, ZValue);
                else if (FaceNumber == 4)
                    holePosition = new Point3d(Part.Length, Position, ZValue);
            }
            else
            {
                if (FaceNumber == 1)
                    holePosition = new Point3d(Position, 0, ZValue);
                else if (FaceNumber == 2)
                    holePosition = new Point3d(Position, Part.Length, ZValue);
                else if (FaceNumber == 3)
                    holePosition = new Point3d(0, Position, ZValue);
                else if (FaceNumber == 4)
                    holePosition = new Point3d(Part.Width, Position, ZValue);
            }
            return holePosition;
        }
    }
}
