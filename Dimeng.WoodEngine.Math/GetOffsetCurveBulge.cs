using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.DatabaseServices;

namespace Dimeng.WoodEngine.Math
{
    public class GetOffsetCurveBulge
    {
        public static double Get(bool IsLeftComp, double r, Polyline line, Point2d TheIntersectPoint, int Index, Point2d StartPoint)
        {
            double oldBulge = line.GetBulgeAt(Index - 1);
            if (oldBulge == 0)
                return 0;//这里可能不对，可能有两个交点，而默认为一个交点
            else
            {
                double delta = System.Math.Atan(System.Math.Abs(oldBulge)) * 2;

                //这里计算新的半径时，要考虑方向因素，可能新半径会更小，也可能会更大
                //取决于路径的偏移方向，以及圆心的位置
                double newRadius = line.GetPoint2dAt(Index - 1).GetDistanceTo(line.GetPoint2dAt(Index)) / 2 / System.Math.Sin(delta);//新弧的半径
                if (IsLeftComp)
                {
                    if (oldBulge < 0)
                        newRadius += r;
                    else newRadius -= r;
                }
                else
                {
                    if (oldBulge > 0)
                        newRadius += r;
                    else newRadius -= r;
                }

                double newChord = StartPoint.GetDistanceTo(TheIntersectPoint);
                double newBulge = System.Math.Tan(
                    System.Math.Asin(newChord / 2 / newRadius) / 2
                    )
                    * line.GetBulgeAt(Index - 1) / System.Math.Abs(line.GetBulgeAt(Index - 1));
                return -newBulge;
            }
        }
    }
}
