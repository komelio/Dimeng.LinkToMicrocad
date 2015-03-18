using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dimeng.LinkToMicrocad.Logging;
using Dimeng.WoodEngine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.LinkToMicrocad.Drawing
{
    public class PartDrawer
    {
        Database db;

        public PartDrawer(Database db)
        {
            this.db = db;
        }

        public void Draw(Part part)
        {
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                LayerHelper.SetLayer(db, part.Material.Name);

                Solid3d panel = drawPart(part);
                btr.AppendEntity(panel);
                trans.AddNewlyCreatedDBObject(panel, true);

                trans.Commit();
            }
        }

        private static Solid3d drawPart(Part part)
        {
            Logger.GetLogger().Debug(string.Format("Drawing part {0}/{1}/{2}/{3}/{4}",
                        part.PartName, part.Width, part.Length, part.Thickness, part.Material));
            Logger.GetLogger().Debug(string.Format("-- Positions: {0}",
                        part.CenterVector));

            Solid3d panel = (new BaseSolidStructer()).Draw(part);
            (new VDrillDrawer()).Draw(panel, part);
            (new HDrillDrawer()).Draw(panel, part);

            panel.TransformBy(Matrix3d.Rotation(part.XRotation * Math.PI / 180, Vector3d.XAxis, Point3d.Origin));
            panel.TransformBy(Matrix3d.Rotation(part.YRotation * Math.PI / 180, Vector3d.YAxis, Point3d.Origin));
            panel.TransformBy(Matrix3d.Rotation(part.ZRotation * Math.PI / 180, Vector3d.ZAxis, Point3d.Origin));

            panel.TransformBy(Matrix3d.Displacement(part.CenterVector));

            panel.Layer = part.Material.Name;

            return panel;
        }
    }
}
