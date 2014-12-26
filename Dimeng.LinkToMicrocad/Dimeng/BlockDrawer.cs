using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dimeng.LinkToMicrocad
{
    internal class BlockDrawer
    {
        double width;
        double height;
        double depth;
        string filePath;

        public BlockDrawer(double width, double height, double depth, string folderPath)
        {
            this.width = width;
            this.height = height;
            this.depth = depth;
            filePath = Path.Combine(folderPath, "temp.dwg");
        }

        public void DrawAndSaveAs()
        {
            if (File.Exists(filePath))
                throw new Exception("已经存在temp.dwg，请保证已经删除");

            using (Database db = new Database())
            using (Transaction acTrans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)acTrans.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)acTrans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                Solid3d solid = new Solid3d();
                solid.CreateBox(width, depth, height);

                solid.TransformBy(Matrix3d.Displacement(new Vector3d(width / 2, -depth / 2, height / 2)));

                btr.AppendEntity(solid);
                acTrans.AddNewlyCreatedDBObject(solid, true);

                acTrans.Commit();

                db.SaveAs(filePath, DwgVersion.Newest);
            }
        }
    }
}
