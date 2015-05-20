using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
﻿using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dimeng.WoodEngine.Entities;
using Dimeng.LinkToMicrocad.Logging;

namespace Dimeng.LinkToMicrocad.Drawing
{
    public class PartBendDrawer
    {
        Point3d startPoint;
        Part part;
        Database db;
        Vector3d offsetVector;

        public PartBendDrawer(Part part, Point3d startPoint, Database db, Vector3d offsetVector)
        {
            this.part = part;
            this.startPoint = startPoint;
            this.db = db;

            if (!part.IsBend)
            {
                throw new Exception("必须是弯曲板件!");
            }
        }

        public void Draw()
        {
            Logger.GetLogger().Debug("Start drawing bending part:" + part.PartName);
            LayerHelper.SetLayer(db, part.Material.Name);
            try
            {
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                    Solid3d partSolid = getBendPartSolid(part);
                    partSolid.Layer = part.Material.Name;

                    //旋转              
                    partSolid.TransformBy(Matrix3d.Rotation(part.XRotation * System.Math.PI / 180, Vector3d.XAxis, Point3d.Origin));//因为是先旋转再位移，所以solid3d默认是画在0点的，旋转也以0点为中心        
                    partSolid.TransformBy(Matrix3d.Rotation(part.YRotation * System.Math.PI / 180, Vector3d.YAxis, Point3d.Origin));
                    partSolid.TransformBy(Matrix3d.Rotation(part.ZRotation * System.Math.PI / 180, Vector3d.ZAxis, Point3d.Origin));

                    //移动               
                    Vector3d moveVector = new Vector3d(startPoint.X, startPoint.Y, startPoint.Z);

                    Point3d basePoint = part.GetPartPointPositionByNumber(1);//MV将点1作为弧形的圆心,无论基准点是什么
                    partSolid.TransformBy(Matrix3d.Displacement(new Vector3d(basePoint.X, basePoint.Y, basePoint.Z) + moveVector));

                    partSolid.TransformBy(Matrix3d.Displacement(offsetVector));

                    btr.AppendEntity(partSolid);
                    trans.AddNewlyCreatedDBObject(partSolid, true);

                    trans.Commit();
                }
            }
            catch
            {
                throw;
            }
        }

        private Solid3d getBendPartSolid(Part part)
        {
            Solid3d solid = new Solid3d();
            double R1 = part.BendingInfo.Radius;
            double r1 = R1 - part.Thickness;
            double arcLength;
            double width;//对应非弯曲方向的长度      
            double angle;
            double inclinationAngle = part.BendingInfo.Angle / 180 * System.Math.PI;//倾角,转换为弧度  

            if (!part.BendingInfo.IsLongSide)
            {
                arcLength = part.Width;
                width = part.Length;
            }
            else
            {
                arcLength = part.Length;
                width = part.Width;
            }

            //计算拱形的弯曲角度      
            //TODO：需要处理的情况，如果弧长太长，超过了一周的长度，要怎么处理？   
            //TODO:现有的算法下，处理出来的结果和MV是相反的       
            angle = arcLength / R1;
            angle = CostDownAngle(angle);
            Polyline baseSectionPolyline = GetSection(angle, R1, r1);
            if (!part.BendingInfo.IsLongSide)
            {
                baseSectionPolyline.TransformBy(Matrix3d.Rotation(System.Math.PI / 2, Vector3d.XAxis, Point3d.Origin));
                baseSectionPolyline.TransformBy(Matrix3d.Rotation(System.Math.PI / 2, Vector3d.ZAxis, Point3d.Origin));
            }
            else
            {
                baseSectionPolyline.TransformBy(Matrix3d.Rotation(System.Math.PI / 2, Vector3d.XAxis, Point3d.Origin));
                //baseSectionPolyline.TransformBy(Matrix3d.Rotation(System.Math.PI / 2, Vector3d.ZAxis, Point3d.Origin));    
            }

            double R2 = R1 + width * System.Math.Abs(System.Math.Tan(inclinationAngle));
            double r2 = R2 - part.Thickness;
            Polyline EngageSectionPolyline = GetSection(angle, R2, r2);
            if (!part.BendingInfo.IsLongSide)
            {
                EngageSectionPolyline.TransformBy(Matrix3d.Rotation(System.Math.PI / 2, Vector3d.XAxis, Point3d.Origin));
                EngageSectionPolyline.TransformBy(Matrix3d.Rotation(System.Math.PI / 2, Vector3d.ZAxis, Point3d.Origin));
                EngageSectionPolyline.TransformBy(Matrix3d.Displacement(new Vector3d(width, 0, 0)));
            }
            else
            {
                EngageSectionPolyline.TransformBy(Matrix3d.Rotation(System.Math.PI / 2, Vector3d.XAxis, Point3d.Origin));
                EngageSectionPolyline.TransformBy(Matrix3d.Displacement(new Vector3d(0, width, 0)));
            }

            Entity[] entities = new Entity[] { baseSectionPolyline, EngageSectionPolyline };
            Entity[] entities2 = new Entity[0];

            solid.CreateLoftedSolid(entities, entities2, null, new LoftOptions());

            return solid;
        }
        private static double CostDownAngle(double angle)
        {
            if (angle > System.Math.PI * 2)
                return CostDownAngle(angle - System.Math.PI * 2);
            else return angle;
        }
        private static Polyline GetSection(double angle, double R1, double r1)
        {
            Polyline polyline = new Polyline();
            Point2d pt1 = new Point2d(-r1 * System.Math.Sin(angle / 2), r1 * System.Math.Cos(angle / 2));
            Point2d pt2 = new Point2d(-R1 * System.Math.Sin(angle / 2), R1 * System.Math.Cos(angle / 2));
            Point2d pt4 = new Point2d(r1 * System.Math.Sin(angle / 2), r1 * System.Math.Cos(angle / 2));
            Point2d pt3 = new Point2d(R1 * System.Math.Sin(angle / 2), R1 * System.Math.Cos(angle / 2));
            polyline.AddVertexAt(0, pt1, 0, 0, 0);
            polyline.AddVertexAt(1, pt2, -System.Math.Tan(angle / 4), 0, 0);
            polyline.AddVertexAt(2, pt3, 0, 0, 0);
            polyline.AddVertexAt(3, pt4, System.Math.Tan(angle / 4), 0, 0);
            polyline.Closed = true;
            return polyline;
        }

    }
}
