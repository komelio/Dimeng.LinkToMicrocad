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

            prepareData(product);//TODO:not here

            using (Database db = new Database())
            using (Transaction acTrans = db.TransactionManager.StartTransaction())
            {
                Database oDb = HostApplicationServices.WorkingDatabase;
                HostApplicationServices.WorkingDatabase = db;//重要，否则会导致很多问题，比如图层加入了却找不到

                BlockTable bt = (BlockTable)acTrans.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)acTrans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                Logger.GetLogger().Debug("Drawing product elements....");
                Logger.GetLogger().Debug("Parts:" + product.Parts.Count.ToString());
                foreach (var part in product.Parts)
                {
                    LayerHelper.SetLayer(db, part.LayerName3D);
                    //LayerHelper.SetLayer(db, "DOOR");

                    Solid3d panel = drawPart(part);
                    panel.Layer = part.LayerName3D;
                    //panel.Layer = "DOOR";

                    btr.AppendEntity(panel);
                    acTrans.AddNewlyCreatedDBObject(panel, true);
                }

                foreach (var sub in product.Subassemblies)
                {
                    Logger.GetLogger().Debug(string.Format("Drawing subassembly {0} elements....", sub.Description));
                    Logger.GetLogger().Debug("Parts:" + sub.Parts.Count.ToString());
                    foreach (var part in sub.Parts)
                    {
                        LayerHelper.SetLayer(db, part.LayerName3D);

                        Solid3d panel = drawPart(part);
                        panel.Layer = part.LayerName3D;

                        btr.AppendEntity(panel);
                        acTrans.AddNewlyCreatedDBObject(panel, true);
                    }
                }

                acTrans.Commit();

                HostApplicationServices.WorkingDatabase = oDb;//重要，否则会导致很多问题，比如图层加入了却找不到
                //另外先改回，在保存，否则会导致报错
                db.SaveAs(savePath, DwgVersion.Newest);

            }
        }

        private static Solid3d drawPart(Part part)
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
            return panel;
        }

        private static void prepareData(Product product)
        {
            product.Parts.ForEach(it => it.CalculateLocationInfo(Point3d.Origin, 0));

            foreach (var sub in product.Subassemblies)
            {
                sub.Parts.ForEach(it => it.CalculateLocationInfo(new Point3d(sub.XOrigin, sub.YOrigin, sub.ZOrigin), sub.Rotation));
            }
        }
    }
}
