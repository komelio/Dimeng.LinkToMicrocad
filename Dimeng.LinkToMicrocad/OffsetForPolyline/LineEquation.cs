using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using System.Collections.Generic;

namespace Offset
{
    public class LineEquation:Equation
    {
        public double a{ get;private set;}

        public double b{ get;private set;}

        public double t{ get;private set;}

        public bool isVerticalAboutXAxis{ get;private set;}

        public LineEquation(double aa, double bb)
        {
            a = aa;
            b = bb;
            isVerticalAboutXAxis = false;
        }

        public LineEquation(double tt)
        {
            t = tt;
            isVerticalAboutXAxis = true;
        }

        public LineEquation(SEG segment, Point2d end)
        {
            if (Math.Abs(segment.point.X-end.X)<=SomeConstants.ERRORLIMIT)
            {
                t = end.X;
                isVerticalAboutXAxis = true;
            }
            else
            {
                a = (segment.point.Y - end.Y) / (segment.point.X - end.X);
                b = segment.point.Y - a * segment.point.X;
                isVerticalAboutXAxis = false;
            }
        }

        public override string ToString()
        {
            if (isVerticalAboutXAxis)
            {
                return string.Format("x={0}", t);
            }
            else
            {
                if (b >= 0)
                    return string.Format("y={0}x+{1}", a, b);
                else
                    return string.Format("y={0}x-{1}", a, -b);
            }
        }

        public override bool HasIPWith(LineEquation line)
        {
            if (isVerticalAboutXAxis && line.isVerticalAboutXAxis)
            {
                if (Math.Abs(t-line.t)<=SomeConstants.ERRORLIMIT)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (isVerticalAboutXAxis ^ line.isVerticalAboutXAxis)
                {
                    return true;
                }
                else
                {
                    if (Math.Abs(a-line.a)>SomeConstants.ERRORLIMIT)
                    {
                        return true;
                    }
                    else
                    {
                        if (Math.Abs(b - line.b) > SomeConstants.ERRORLIMIT)
                            return false;
                        else
                            return true;
                    }
                }
            }
        }

        public override bool HasIPWith(CircleEquation circle)
        {
            if (this.isVerticalAboutXAxis == true)
            {
                double distance = Math.Abs(circle.a - t);
                if (distance <= circle.r)
                    return true;
                else
                    return false;
            }
            double AA = 1 + a * a;
            double BB = 2 * a * b + circle.D + a * circle.E;
            double CC = b * b + b * circle.E + circle.F;
            return B2_4AC(AA, BB, CC);
        }

        public bool IsOverlappingWith(LineEquation line)
        {
            if (isVerticalAboutXAxis && line.isVerticalAboutXAxis)
            {
                if (Math.Abs(t-line.t)<=SomeConstants.ERRORLIMIT)
                    return true;
            }
            else
            {
                if ((!isVerticalAboutXAxis) && (!line.isVerticalAboutXAxis))
                {
                    if ((Math.Abs(a-line.a)<=SomeConstants.ERRORLIMIT )&& (Math.Abs(b-line.b)<=SomeConstants.ERRORLIMIT))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public double GetYOflineEquation(double x)
        {
            return a * x + b;
        }

        public override Point2d[] IPWith(LineEquation line)
        {
            
                 if (this.isVerticalAboutXAxis)
                    {
                        double xxip = this.t;
                        double yyip = line.GetYOflineEquation(xxip);
                        Point2d iip = new Point2d(xxip, yyip);
                        Point2d[] ipss = new Point2d[] { iip };
                        return ipss;
                    }
                    if (line.isVerticalAboutXAxis)
                    {
                        double xxxip = line.t;
                        double yyyip = this.GetYOflineEquation(xxxip);
                        Point2d iiip = new Point2d(xxxip, yyyip);
                        Point2d[] ipsss = new Point2d[] { iiip };
                        return ipsss;
                    }
                    double xip = (line.b - b) / (a - line.a);
                    double yip = GetYOflineEquation(xip);
                    Point2d ip = new Point2d(xip, yip);
                    Point2d[] ips = new Point2d[] { ip };
                    return ips;
         
        }

        public override Point2d[] IPWith(CircleEquation circle)
        {
            
                if (this.isVerticalAboutXAxis == true)
                {
                    double ipy1, ipy2;
                    double A = 1;
                    double B = circle.E;
                    double C = this.t * this.t + circle.D * this.t + circle.F;
                    GetX1X2(A, B, C, out ipy1, out ipy2);
                    Point2d ip11 = new Point2d(this.t, ipy1);
                    Point2d ip22 = new Point2d(this.t, ipy2);
                    Point2d[] ipss = new Point2d[] { ip11, ip22 };
                    return ipss;
                }
                double x1ip, x2ip, y1ip, y2ip;
                double AA = 1 + a * a;
                double BB = 2 * a * b + circle.D + a * circle.E;
                double CC = b * b + b * circle.E + circle.F;
                GetX1X2(AA, BB, CC, out x1ip, out x2ip);
                y1ip = GetYOflineEquation(x1ip);
                y2ip = GetYOflineEquation(x2ip);
                Point2d ip1 = new Point2d(x1ip, y1ip);
                Point2d ip2 = new Point2d(x2ip, y2ip);
                Point2d[] ips = new Point2d[] { ip1, ip2 };
                return ips;
 
        }
    }
}
