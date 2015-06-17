using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using System;

namespace Offset
{
    public class SEG
    {
        
        public Point2d point
        {
            get;
            set;
        }

        public double bulge
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SEG) || obj==null)
                return false;
            else
            {
                SEG seg = obj as SEG;
                return point == seg.point && bulge == seg.bulge;
            }
        }

        public SEG(Point2d p, double b)
        {
            point = p;
            bulge = b;
        }

        public void AddPoint(Point2d pToAdd, Point2d endofthis, out SEG newSeg1, out SEG newSeg2)
        {
                double Ang1=0,Ang2=0,bulge1=0,bulge2=0;
                if (this.bulge == 0)
                {
                    
                    bulge1 = 0;
                    bulge2 = 0;
                }
                else
                {
                    CircleEquation cirSeg = new CircleEquation(this, endofthis);
                    Point2d centerSeg = new Point2d(cirSeg.a, cirSeg.b);
                    Vector2d cts=VectorCalculations.VectorOfTwoPoints(centerSeg,this.point);
                    Vector2d ctp = VectorCalculations.VectorOfTwoPoints(centerSeg, pToAdd);
                    Vector2d cte = VectorCalculations.VectorOfTwoPoints(centerSeg, endofthis);
                    
                    double angvcs = VectorCalculations.GetAngleOfVector(cts);
                    double angvce = VectorCalculations.GetAngleOfVector(cte);
                    double angvcp = VectorCalculations.GetAngleOfVector(ctp);
                    if (this.bulge < 0)
                    {
                        if (angvcs > angvce)
                        {
                            Ang2 = angvcp - angvce;
                            Ang1 = angvcs - angvcp;
                        }
                        else
                        {
                            if (angvcp < angvcs)
                            {
                                Ang1 = angvcs - angvcp;
                                Ang2 = 2 * Math.PI - angvce + angvcp;
                            }
                            else
                            {
                                Ang1 = angvcs + 2 * Math.PI - angvcp;
                                Ang2 = angvcp - angvce;
                            }
                        }
                        bulge1 = -Math.Tan(Ang1 / 4);
                        bulge2 = -Math.Tan(Ang2 / 4);
                    }
                    else
                    {
                        if (angvce < angvcs)
                        {
                            if (angvcp < angvce)
                            {
                                Ang1 = angvcp + 2 * Math.PI - angvcs;
                                Ang2 = angvce - angvcp;
                            }
                            else
                            {
                                Ang1 = angvcp - angvcs;
                                Ang2 = angvce + 2 * Math.PI - angvcp;
                            }
                        }
                        else
                        {
                            Ang1 = angvcp - angvcs;
                            Ang2 = angvce - angvcp;
                        }
                        bulge1 = Math.Tan(Ang1 / 4);
                        bulge2 = Math.Tan(Ang2 / 4);
                    }
                }
                newSeg1 = new SEG(this.point, bulge1);
                newSeg2 = new SEG(pToAdd, bulge2);
        }

        //public SEG ExtendeSeg(Point2d extendp)
        //{
            
        //}

        public override string ToString()
        {
            return "\n" + point.ToString() + "|Bulge:" + bulge + "\n";
        }

        public Point2d[] GetIPS(Point2d endofthis, SEG s, Point2d endofs)
        {
            if (this.bulge == 0)
            {
                LineEquation thisline = new LineEquation(this, endofthis);
                if (s.bulge == 0)
                {
                    LineEquation sline = new LineEquation(s, endofs);
                    return thisline.IPWith(sline);
                }
                else
                {
                    CircleEquation cline = new CircleEquation(s, endofs);
                    return thisline.IPWith(cline);
                }
            }
            else
            {
                CircleEquation thiscir = new CircleEquation(this, endofthis);
                if (s.bulge == 0)
                {
                    LineEquation sline = new LineEquation(s, endofs);
                    return thiscir.IPWith(sline);
                }
                else
                {
                    CircleEquation cline = new CircleEquation(s, endofs);
                    return thiscir.IPWith(cline);
                }
            }
        }

        public Point2d midPointOfSeg(Point2d endofmid,Point2d endofthisseg)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            if (this.bulge == 0)
            {
                return VectorCalculations.MidPoint(this.point, endofmid);
            }
            else
            {
                CircleEquation cirthis = new CircleEquation(this, endofthisseg);
                Point2d circenter = new Point2d(cirthis.a, cirthis.b);
                Vector2d vceom = VectorCalculations.VectorOfTwoPoints(circenter, endofmid);
                Vector2d vcs = VectorCalculations.VectorOfTwoPoints(circenter, this.point);
                double radius = VectorCalculations.NormOfVector(vcs);
                double angvcem = VectorCalculations.GetAngleOfVector(vceom);
                double angvcs = VectorCalculations.GetAngleOfVector(vcs);
                
                double angvcm = 0;
                if (this.bulge < 0)
                {
                    if (angvcem < angvcs)
                    {
                        angvcm = angvcem + (angvcs - angvcem) / 2;
                    }
                    else
                    {
                        double angtoadd = (2 * Math.PI - (angvcem-angvcs)) / 2;
                        angvcm = angvcem + angtoadd;
                    }
                }
                else
                {
                    if (angvcem < angvcs)
                    {
                        double angtoadd = (2 * Math.PI - (angvcs-angvcem)) / 2;
                        angvcm = angvcs + angtoadd;
                    }
                    else
                    {
                        angvcm = angvcs + (angvcem - angvcs) / 2;
                    }
                }
                return VectorCalculations.GetVectorFromAngNorm(angvcm-2*Math.PI, radius,circenter);
            }
        }
    }
}
