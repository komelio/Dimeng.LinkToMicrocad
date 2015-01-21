using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dimeng.LinkToMicrocad.Logging;
using Dimeng.WoodEngine.Entities;
using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dimeng.LinkToMicrocad.Drawing
{
    public class ProductDrawer
    {
        public void DrawAndSaveAsDWG(Product product, IWorkbookSet bookSet, string savePath)
        {
            Logger.GetLogger().Info("Start drawing the cad block.");

            if (File.Exists(savePath))
            { throw new Exception("已经存在temp.dwg，请保证已经删除"); }

            product.Parts.ForEach(it => it.CalculateLocationInfo(Point3d.Origin, 0));//TODO:Not here

            using (Database db = new Database())
            using (Transaction acTrans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)acTrans.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)acTrans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                foreach (var part in product.Parts)
                {
                    Logger.GetLogger().Debug(string.Format("Drawing part {0}/{1}/{2}/{3}/{4}",
                        part.PartName, part.Width, part.Length, part.Thickness, part.Material));
                    Logger.GetLogger().Debug(string.Format("-- Positions: {0}",
                        part.CenterVector));
                    
                    Solid3d panel = new Solid3d();
                    panel.CreateBox(part.Length, part.Width, part.Thickness);

                    panel.TransformBy(Matrix3d.Rotation(part.XRotation * Math.PI / 180, Vector3d.XAxis, Point3d.Origin));
                    panel.TransformBy(Matrix3d.Rotation(part.YRotation * Math.PI / 180, Vector3d.YAxis, Point3d.Origin));
                    panel.TransformBy(Matrix3d.Rotation(part.ZRotation * Math.PI / 180, Vector3d.ZAxis, Point3d.Origin));

                    panel.TransformBy(Matrix3d.Displacement(part.CenterVector));

                    btr.AppendEntity(panel);
                    acTrans.AddNewlyCreatedDBObject(panel, true);
                }

                acTrans.Commit();

                db.SaveAs(savePath, DwgVersion.Newest);
            }
        }
    }
}
