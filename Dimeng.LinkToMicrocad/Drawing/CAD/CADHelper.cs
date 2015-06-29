﻿using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dimeng.LinkToMicrocad.Drawing.CAD
{
    public class CADHelper
    {
        public static Entity AddDwgEntities(string path, Transaction Trans)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database CurDb = doc.Database;
            ObjectIdCollection collection = new ObjectIdCollection();

            //看来不能直接新创建一个Transaction来获取结果，因为返回之后这个Trans就结束了   
            //using (Transaction Trans = CurDb.TransactionManager.StartTransaction())     
            using (Database OpenDb = new Database(false, true))
            {
                OpenDb.ReadDwgFile(path, FileShare.ReadWrite, true, "");
                using (Transaction openTrans = OpenDb.TransactionManager.StartTransaction())
                {
                    BlockTable openBt = (BlockTable)openTrans.GetObject(OpenDb.BlockTableId, OpenMode.ForWrite);
                    BlockTableRecord btr = (BlockTableRecord)openTrans.GetObject(openBt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                    foreach (var a in btr)
                    {
                        Entity entity = (Entity)openTrans.GetObject(a, OpenMode.ForWrite);
                        collection.Add(a);
                    }
                    openTrans.Commit();
                }

                BlockTable Bt = (BlockTable)Trans.GetObject(CurDb.BlockTableId, OpenMode.ForWrite);
                BlockTableRecord btr2 = (BlockTableRecord)Trans.GetObject(Bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                IdMapping map = new IdMapping();
                CurDb.WblockCloneObjects(collection, btr2.ObjectId, map, DuplicateRecordCloning.Replace, false);//把dwg文件里的文件拷贝到当前的Database       

                foreach (IdPair id in map)
                {
                    //跳过那些被删掉的东西，我也不知道为什么会被拷贝近来
                    if (id.Value.IsErased)
                    {
                        continue;
                    }

                    BlockTableRecord btr = Trans.GetObject(id.Value, OpenMode.ForWrite) as BlockTableRecord;
                    if (btr != null)
                    {
                        foreach (var a in btr)
                        {
                            Entity entity = Trans.GetObject(a, OpenMode.ForWrite) as Entity;
                            if (entity is Polyline || entity is Polyline2d)
                            {
                                return entity;
                            }
                        }
                    }

                }
                throw new System.Exception("未找到属于该dwg的多段线截面数据" + path);
            }
        }

        public static void AddDwgAsBlock(Database CurDb, FileInfo file, Point3d pt, Vector3d xaxis, Vector3d yaxis, Vector3d zaxis, double angle)
        {
            string blockName = file.Name.Substring(0, file.Name.LastIndexOf(file.Extension));

            ObjectId id;
            using (Transaction Trans = CurDb.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)Trans.GetObject(CurDb.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)Trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                if (bt.Has(blockName))
                {
                    id = Trans.GetObject(bt[blockName], OpenMode.ForRead).Id;
                }
                else
                {
                    using (Database OpenDb = new Database(false, true))
                    {
                        OpenDb.ReadDwgFile(file.FullName, FileShare.ReadWrite, true, "");
                        id = CurDb.Insert(blockName, OpenDb, true);
                    }
                }

                BlockReference br = new BlockReference(Point3d.Origin, id);

                br.TransformBy(Matrix3d.AlignCoordinateSystem(Point3d.Origin,
                                                              Vector3d.XAxis,
                                                              Vector3d.YAxis,
                                                              Vector3d.ZAxis,
                                                              Point3d.Origin,
                                                              xaxis,
                                                              yaxis,
                                                              zaxis));
                br.TransformBy(Matrix3d.Rotation(angle / 180 * System.Math.PI, zaxis, Point3d.Origin));
                br.TransformBy(Matrix3d.Displacement(pt - Point3d.Origin));

                btr.AppendEntity(br);
                Trans.AddNewlyCreatedDBObject(br, true);

                Trans.Commit();
            }
        }

        public static bool CheckPolylineSelfIntersect(Polyline polyline)
        {
            using (DBObjectCollection entities = new DBObjectCollection())
            {
                polyline.Explode(entities);
                for (int i = 0; i < entities.Count; ++i)
                {
                    for (int j = i + 1; j < entities.Count; ++j)
                    {
                        Curve curve1 = entities[i] as Curve;
                        Curve curve2 = entities[j] as Curve;
                        Point3dCollection points = new Point3dCollection();
                        curve1.IntersectWith(curve2, Intersect.OnBothOperands, points, IntPtr.Zero, IntPtr.Zero);

                        foreach (Point3d point in points)
                        {
                            // Make a check to skip the start/end points     
                            // since they are connected vertices        
                            if (point == curve1.StartPoint ||
                                point == curve1.EndPoint)
                            {
                                if (point == curve2.StartPoint ||
                                    point == curve2.EndPoint)
                                {
                                    // If two consecutive segments, then skip    
                                    if (j == i + 1)
                                    {
                                        continue;
                                    }
                                }
                            }
                            return true;
                        }
                    }
                    // Need to be disposed explicitely      
                    // since entities are not DB resident       
                    entities[i].Dispose();
                }
            }
            return false;
        }
    }
}
