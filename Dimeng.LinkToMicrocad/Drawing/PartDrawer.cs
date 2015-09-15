using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dimeng.LinkToMicrocad.Logging;
using Dimeng.LinkToMicrocad.Properties;
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
        Vector3d offsetVector;//整体偏移

        public PartDrawer(Database db, Vector3d moveVector)
        {
            this.offsetVector = moveVector;
            this.db = db;
        }

        public void Draw(Part part)
        {
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                LayerHelper.SetLayer(db, part.LayerName3D);

                Solid3d panel = drawPart(part);
                btr.AppendEntity(panel);
                trans.AddNewlyCreatedDBObject(panel, true);

                trans.Commit();
            }
        }

        private Solid3d drawPart(Part part)
        {
            Logger.GetLogger().Debug(string.Format("Drawing part {0}/{1}/{2}/{3}/{4}",
                        part.PartName, part.Width, part.Length, part.Thickness, part.Material));
            Logger.GetLogger().Debug(string.Format("Drawing part {0}/{1}",
                        part.EBW1.Name, part.EBW1.Thickness));
            Logger.GetLogger().Debug(string.Format("Drawing part {0}/{1}",
             part.EBW2.Name, part.EBW2.Thickness));
            Logger.GetLogger().Debug(string.Format("Drawing part {0}/{1}",
            part.EBL1.Name, part.EBL1.Thickness));
            Logger.GetLogger().Debug(string.Format("Drawing part {0}/{1}",
            part.EBL2.Name, part.EBL2.Thickness));
            Logger.GetLogger().Debug(string.Format("-- Position: {0}",
                        part.CenterVector));
            Logger.GetLogger().Debug(string.Format("-- Routings: {0}",
                        part.Routings.Count));
            Logger.GetLogger().Debug(string.Format("-- VDrillings: {0}",
                        part.VDrillings.Count));
            Logger.GetLogger().Debug(string.Format("-- HDrillings: {0}",
                        part.HDrillings.Count));
            Logger.GetLogger().Debug(string.Format("-- Profiles: {0}",
                        part.Profiles.Count));

            Solid3d panel = (new BaseSolidStructer()).Draw(part);
            if (Settings.Default.IsDrawHoles)
            {
                (new VDrillDrawer()).Draw(panel, part);
                (new HDrillDrawer()).Draw(panel, part);
            }
            (new RouteDrawer(Context.GetContext().MVDataContext.GetLatestRelease().CurrentToolFile)).Draw(panel, part);
            (new ProfileDrawer(Context.GetContext().MVDataContext.GetLatestRelease().MicrovellumData + "\\Graphics\\Profiles\\", db)).Draw(panel, part);

            panel.TransformBy(Matrix3d.Rotation(part.TXRotation * System.Math.PI / 180, Vector3d.XAxis, Point3d.Origin));
            panel.TransformBy(Matrix3d.Rotation(part.TYRotation * System.Math.PI / 180, Vector3d.YAxis, Point3d.Origin));
            panel.TransformBy(Matrix3d.Rotation(part.TZRotation * System.Math.PI / 180, Vector3d.ZAxis, Point3d.Origin));

            panel.TransformBy(Matrix3d.Displacement(part.CenterVector));
            panel.TransformBy(Matrix3d.Displacement(this.offsetVector));

            panel.Layer = part.LayerName3D;

            return panel;
        }
    }
}