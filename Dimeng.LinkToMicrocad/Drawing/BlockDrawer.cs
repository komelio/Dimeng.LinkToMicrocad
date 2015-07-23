using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dimeng.LinkToMicrocad.Logging;
using Dimeng.WoodEngine.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dimeng.LinkToMicrocad.Drawing
{
    public class BlockDrawer
    {
        Part part;
        Database db;
        string blockPath;

        public BlockDrawer(Part part, Database db, string blockPath)
        {
            this.db = db;
            this.part = part;
            this.blockPath = blockPath;
        }

        public void Draw()
        {
            //return;
            string blockQualifiedFileName = Path.GetFileNameWithoutExtension(this.blockPath);

            using (Transaction tran = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)tran.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)tran.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);

                if (!bt.Has(blockQualifiedFileName))
                {
                    using (Database tmpDb = new Database(false, true))
                    {
                        tmpDb.ReadDwgFile(this.blockPath, System.IO.FileShare.Read, true, "");
                        db.Insert(blockQualifiedFileName, tmpDb, true);
                    }
                }

                BlockReference br = new BlockReference(Point3d.Origin, bt[blockQualifiedFileName]);
                br.ScaleFactors = new Scale3d(part.Length / 400, part.Width / 300, part.Thickness / 18);
                br.TransformBy(Matrix3d.Rotation(part.TXRotation * System.Math.PI / 180, Vector3d.XAxis, Point3d.Origin));
                br.TransformBy(Matrix3d.Rotation(part.TYRotation * System.Math.PI / 180, Vector3d.YAxis, Point3d.Origin));
                br.TransformBy(Matrix3d.Rotation(part.TZRotation * System.Math.PI / 180, Vector3d.ZAxis, Point3d.Origin));
                br.TransformBy(Matrix3d.Displacement(part.CenterVector));

                btr.AppendEntity(br);
                tran.AddNewlyCreatedDBObject(br, true);
                tran.Commit();

            }
            //ObjectId id = db.Insert("blockDoor", tmpDb, true);

            //using (Transaction tran = db.TransactionManager.StartTransaction())
            //{
            //    BlockReference br = (BlockReference)tran.GetObject(id, OpenMode.ForWrite);
            //    br.ScaleFactors = new Scale3d(1.0, 1.5, 1);

            //    tran.Commit();
            //}

        }
    }
}
