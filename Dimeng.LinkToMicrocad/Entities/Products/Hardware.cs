using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities
{
    public class Hardware
    {
        public string Name { get; set; }
        public int Qty { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Depth { get; set; }
        public double XOrigin { get; set; }
        public double YOrigin { get; set; }
        public double ZOrigin { get; set; }
        public double AssociatedRotation { get; set; }
        public string Comment { get; set; }
        public string Comment2 { get; set; }
        public string Comment3 { get; set; }
        public HardwareType HardwareType { get; set; }

        public double TXOrigin { get; set; }
        public double TYOrigin { get; set; }
        public double TZOrigin { get; set; }
        public double TAssociatedRotation { get; set; }

        public bool OnTopFace { get; private set; }

        public bool IsHaveAssocaitedRotation { get; set; }

        public Part AssociatedPart { get; private set; }

        public override string ToString()
        {
            return Name;
        }

        public void SetLocation(Point3d orign, double rotation)
        {
            TXOrigin = XOrigin + orign.X;
            TYOrigin = YOrigin + orign.Y;
            TZOrigin = ZOrigin + orign.Z;
            TAssociatedRotation = AssociatedRotation + rotation;
        }

        public bool IsInPart(Part p)
        {
            //判断是否在一个板件的内部
            //判断方法：以板件的机加工原点为基准，判断五金的坐标相对此点的相对坐标
            //如果坐标在板件外部，说明这个点
            Point3d pt = new Point3d(this.TXOrigin, this.TYOrigin, this.TZOrigin);
            pt = pt.TransformBy(Matrix3d.AlignCoordinateSystem(
                                                               p.Point4,
                                                               p.MovedOrginXAxis,
                                                               p.MovedOrginYAxis,
                                                               p.MovedOrginZAxis,
                                                               Point3d.Origin,
                                                               Vector3d.XAxis,
                                                               Vector3d.YAxis,
                                                               Vector3d.ZAxis));

            if (pt.Z > 0 || pt.Z < -p.Thickness)
                return false;

            if (!p.MachinePoint.IsRotated)
            {
                if (pt.X >= 0 && pt.X <= p.Length && pt.Y >= 0 && pt.Y <= p.Width)
                {
                    if (pt.Z < -p.Thickness / 2) OnTopFace = false;
                    else OnTopFace = true;

                    return true;
                }
                else return false;
            }
            else
            {
                if (pt.Y >= 0 && pt.Y <= p.Length && pt.X >= 0 && pt.X <= p.Width)
                {
                    if (pt.Z < -p.Thickness / 2) OnTopFace = false;
                    else OnTopFace = true;

                    return true;
                }
                else return false;
            }
        }

        public void FindAssociatedPart(Product p)
        {
            foreach (var part in p.CombinedParts)
            {
                if (this.IsInPart(part))
                {
                    this.AssociatedPart = part;
                    break;
                }
            }
        }
    }
}
