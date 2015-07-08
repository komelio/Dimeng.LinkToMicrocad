using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dimeng.LinkToMicrocad.Drawing.CAD;
using Dimeng.LinkToMicrocad.Logging;
using Dimeng.WoodEngine.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dimeng.LinkToMicrocad.Drawing
{
    public class PartMoldingDrawer
    {
        Part part;
        Database db;
        string mouldingsGraphicPath;
        public PartMoldingDrawer(Part part, Database db, string mouldingsPath)
        {
            this.part = part;
            this.db = db;
            this.mouldingsGraphicPath = mouldingsPath;
        }

        public void Draw()
        {
            Logger.GetLogger().Debug("Start drawing moulding part:" + part.PartName);

            LayerHelper.SetLayer(db, part.Material.Name);

            string filePath = Path.Combine(mouldingsGraphicPath, part.Molding.DWGFileName);
            if (!File.Exists(filePath))
            {
                Logger.GetLogger().Warn(string.Format("Moulding file {0} not found!", filePath));
                return;
            }

            try
            {
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                    Solid3d panel = getSolid(filePath, part.BasePoint);
                    panel.Layer = part.Material.Name;

                    panel.TransformBy(Matrix3d.Rotation(part.TXRotation * System.Math.PI / 180, Vector3d.XAxis, Point3d.Origin));
                    panel.TransformBy(Matrix3d.Rotation(part.TYRotation * System.Math.PI / 180, Vector3d.YAxis, Point3d.Origin));
                    panel.TransformBy(Matrix3d.Rotation(part.TZRotation * System.Math.PI / 180, Vector3d.ZAxis, Point3d.Origin));


                    panel.TransformBy(Matrix3d.Displacement(new Vector3d(part.TXOrigin, part.TYOrigin, part.TZOrigin)));

                    btr.AppendEntity(panel);
                    trans.AddNewlyCreatedDBObject(panel, true);

                    trans.Commit();
                }
            }
            catch
            {
                throw;
            }
        }

        private Solid3d getSolid(string file, int basePoint)
        {
            double offset1 = 0;
            double offset2 = 0;
            double maxY = 0;
            double minY = 0;
            double maxX = 0;
            double minX = 0;

            ObjectIdCollection collection = new ObjectIdCollection();

            //看来不能直接新创建一个Transaction来获取结果，因为返回之后这个Trans就结束了   
            using (Transaction Trans = db.TransactionManager.StartTransaction())
            using (Database OpenDb = new Database(false, true))
            {
                OpenDb.ReadDwgFile(file, FileShare.ReadWrite, true, "");
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

                BlockTable Bt = (BlockTable)Trans.GetObject(db.BlockTableId, OpenMode.ForWrite);
                BlockTableRecord btr2 = (BlockTableRecord)Trans.GetObject(Bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                IdMapping map = new IdMapping();
                db.WblockCloneObjects(collection, btr2.ObjectId, map, DuplicateRecordCloning.Replace, false);//把dwg文件里的文件拷贝到当前的Database       

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
                            if (entity == null)
                            {
                                continue;
                            }

                            Logger.GetLogger().Debug(entity.GetType().ToString());
                            if (entity is Curve)
                            {
                                Curve curve = entity as Curve;
                                Logger.GetLogger().Debug("Curve is closed:" + curve.Closed.ToString());

                                if (curve is Circle)
                                {
                                    Circle circle = curve as Circle;
                                    offset1 = circle.Diameter;
                                    offset2 = circle.Diameter;
                                }
                                else if (curve is Polyline)
                                {
                                    Polyline pl = curve as Polyline;
                                    for (int i = 0; i < pl.NumberOfVertices; i++)
                                    {
                                        double y = pl.GetPoint2dAt(i).Y;
                                        double x = pl.GetPoint2dAt(i).X;

                                        if (i == 0)
                                        {
                                            maxY = y;
                                            minY = y;
                                            maxX = x;
                                            minX = x;
                                        }

                                        if (y > maxY)
                                        {
                                            maxY = y;
                                        }
                                        if (y < minY)
                                        {
                                            minY = y;
                                        }
                                        if (x > maxX)
                                        {
                                            maxX = x;
                                        }
                                        if (x < minX)
                                        {
                                            minX = x;
                                        }
                                    }
                                    offset1 = maxY - minY;
                                    offset2 = maxX - minX;
                                }
                                else if (curve is Polyline2d)
                                {
                                    Polyline2d pl2d = curve as Polyline2d;

                                    int i = 0;
                                    foreach (ObjectId vId in pl2d)
                                    {
                                        Vertex2d v2d = (Vertex2d)Trans.GetObject(vId, OpenMode.ForRead);
                                        double y = v2d.Position.Y;
                                        double x = v2d.Position.X;

                                        if (i == 0)
                                        {
                                            maxY = y;
                                            minY = y;
                                            maxX = x;
                                            minX = x;
                                        }

                                        if (y > maxY)
                                        {
                                            maxY = y;
                                        }
                                        if (y < minY)
                                        {
                                            minY = y;
                                        }
                                        if (x > maxX)
                                        {
                                            maxX = x;
                                        }
                                        if (x < minX)
                                        {
                                            minX = x;
                                        }

                                        i++;
                                    }
                                    offset1 = maxY - minY;
                                    offset2 = maxX - minX;
                                }

                                Logger.GetLogger().Debug("Molding Offset1:" + offset1.ToString());
                                Logger.GetLogger().Debug("Molding Offset2:" + offset2.ToString());

                                Solid3d solid = new Solid3d();
                                SweepOptionsBuilder sob = new SweepOptionsBuilder();
                                sob.Align = SweepOptionsAlignOption.AlignSweepEntityToPath;

                                if (curve is Polyline || curve is Polyline2d)
                                {
                                    using (Polyline newPolyline = CADHelper.RebuildPolyline(curve, db))
                                    {
                                        if (basePoint == 1)
                                        {
                                            solid.CreateExtrudedSolid(newPolyline, new Vector3d(0, 0, part.Length), sob.ToSweepOptions());
                                            solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.YAxis, Point3d.Origin));
                                            solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.XAxis, Point3d.Origin));
                                        }
                                        else if (basePoint == 2)
                                        {
                                            solid.CreateExtrudedSolid(newPolyline, new Vector3d(0, 0, part.Length), sob.ToSweepOptions());
                                            solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.YAxis, Point3d.Origin));
                                            solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.XAxis, Point3d.Origin));
                                            solid.TransformBy(Matrix3d.Displacement(new Vector3d(0, 0, -offset2)));
                                        }
                                        else if (basePoint == 3)
                                        {
                                            solid.CreateExtrudedSolid(newPolyline, new Vector3d(0, 0, part.Length), sob.ToSweepOptions());
                                            solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.YAxis, Point3d.Origin));
                                            solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.XAxis, Point3d.Origin));
                                            solid.TransformBy(Matrix3d.Displacement(new Vector3d(0, -offset1, 0)));
                                        }
                                        else if (basePoint == 4)
                                        {
                                            solid.CreateExtrudedSolid(newPolyline, new Vector3d(0, 0, part.Length), sob.ToSweepOptions());
                                            solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.YAxis, Point3d.Origin));
                                            solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.XAxis, Point3d.Origin));

                                            solid.TransformBy(Matrix3d.Displacement(new Vector3d(0, -offset1, 0)));
                                            solid.TransformBy(Matrix3d.Displacement(new Vector3d(0, 0, -offset2)));

                                        }
                                        else if (basePoint == 5)
                                        {
                                            solid.CreateExtrudedSolid(newPolyline, new Vector3d(0, 0, -part.Length), sob.ToSweepOptions());
                                            solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.YAxis, Point3d.Origin));
                                            solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.XAxis, Point3d.Origin));

                                            solid.TransformBy(Matrix3d.Displacement(new Vector3d(0, -offset1, 0)));
                                        }
                                        else if (basePoint == 6)
                                        {
                                            solid.CreateExtrudedSolid(newPolyline, new Vector3d(0, 0, -part.Length), sob.ToSweepOptions());
                                            solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.YAxis, Point3d.Origin));
                                            solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.XAxis, Point3d.Origin));

                                            solid.TransformBy(Matrix3d.Displacement(new Vector3d(0, -offset1, 0)));
                                            solid.TransformBy(Matrix3d.Displacement(new Vector3d(0, 0, -offset2)));
                                        }
                                        else if (basePoint == 7)
                                        {
                                            solid.CreateExtrudedSolid(newPolyline, new Vector3d(0, 0, -part.Length), sob.ToSweepOptions());
                                            solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.YAxis, Point3d.Origin));
                                            solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.XAxis, Point3d.Origin));
                                        }
                                        else//base point 8
                                        {
                                            solid.CreateExtrudedSolid(newPolyline, new Vector3d(0, 0, -part.Length), sob.ToSweepOptions());
                                            solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.YAxis, Point3d.Origin));
                                            solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.XAxis, Point3d.Origin));
                                            solid.TransformBy(Matrix3d.Displacement(new Vector3d(0, 0, -offset2)));
                                        }
                                    }
                                }
                                else//针对圆
                                {
                                    if (basePoint == 1)
                                    {
                                        solid.CreateExtrudedSolid(curve, new Vector3d(0, 0, part.Length), sob.ToSweepOptions());
                                        solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.YAxis, Point3d.Origin));
                                        solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.XAxis, Point3d.Origin));
                                    }
                                    else if (basePoint == 2)
                                    {
                                        solid.CreateExtrudedSolid(curve, new Vector3d(0, 0, part.Length), sob.ToSweepOptions());
                                        solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.YAxis, Point3d.Origin));
                                        solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.XAxis, Point3d.Origin));
                                        solid.TransformBy(Matrix3d.Displacement(new Vector3d(0, 0, -offset2)));
                                    }
                                    else if (basePoint == 3)
                                    {
                                        solid.CreateExtrudedSolid(curve, new Vector3d(0, 0, part.Length), sob.ToSweepOptions());
                                        solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.YAxis, Point3d.Origin));
                                        solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.XAxis, Point3d.Origin));
                                        solid.TransformBy(Matrix3d.Displacement(new Vector3d(0, -offset1, 0)));
                                    }
                                    else if (basePoint == 4)
                                    {
                                        solid.CreateExtrudedSolid(curve, new Vector3d(0, 0, part.Length), sob.ToSweepOptions());
                                        solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.YAxis, Point3d.Origin));
                                        solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.XAxis, Point3d.Origin));

                                        solid.TransformBy(Matrix3d.Displacement(new Vector3d(0, -offset1, 0)));
                                        solid.TransformBy(Matrix3d.Displacement(new Vector3d(0, 0, -offset2)));

                                    }
                                    else if (basePoint == 5)
                                    {
                                        solid.CreateExtrudedSolid(curve, new Vector3d(0, 0, -part.Length), sob.ToSweepOptions());
                                        solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.YAxis, Point3d.Origin));
                                        solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.XAxis, Point3d.Origin));

                                        solid.TransformBy(Matrix3d.Displacement(new Vector3d(0, -offset1, 0)));
                                    }
                                    else if (basePoint == 6)
                                    {
                                        solid.CreateExtrudedSolid(curve, new Vector3d(0, 0, -part.Length), sob.ToSweepOptions());
                                        solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.YAxis, Point3d.Origin));
                                        solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.XAxis, Point3d.Origin));

                                        solid.TransformBy(Matrix3d.Displacement(new Vector3d(0, -offset1, 0)));
                                        solid.TransformBy(Matrix3d.Displacement(new Vector3d(0, 0, -offset2)));
                                    }
                                    else if (basePoint == 7)
                                    {
                                        solid.CreateExtrudedSolid(curve, new Vector3d(0, 0, -part.Length), sob.ToSweepOptions());
                                        solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.YAxis, Point3d.Origin));
                                        solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.XAxis, Point3d.Origin));
                                    }
                                    else//base point 8
                                    {
                                        solid.CreateExtrudedSolid(curve, new Vector3d(0, 0, -part.Length), sob.ToSweepOptions());
                                        solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.YAxis, Point3d.Origin));
                                        solid.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.XAxis, Point3d.Origin));
                                        solid.TransformBy(Matrix3d.Displacement(new Vector3d(0, 0, -offset2)));
                                    }
                                }

                                entity.Erase();

                                Trans.Commit();
                                return solid;
                                //}
                            }
                            //entity.Erase();
                        }
                        Trans.Commit();
                    }
                }
            }

            throw new Exception("Invald dwg file:" + file);
        }
    }
}
