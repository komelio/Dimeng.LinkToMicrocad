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
        Vector3d offsetVector;
        IMVLibrary library;
        public ProductDrawer(Vector3d offsetVector, IMVLibrary library)
        {
            this.library = library;
            this.offsetVector = offsetVector;
        }

        public void DrawAndSaveAsDWG(Product product, IWorkbookSet bookSet, string savePath)
        {
            Logger.GetLogger().Info("Start drawing the cad block.");

            if (File.Exists(savePath))
            { throw new Exception("已经存在temp.dwg，请保证已经删除"); }

            Logger.GetLogger().Debug("Drawing product elements....");
            Logger.GetLogger().Debug("Parts:" + product.Parts.Count.ToString());

            using (Database db = new Database())
            {
                Database oDb = HostApplicationServices.WorkingDatabase;
                HostApplicationServices.WorkingDatabase = db;//重要，否则会导致很多问题，比如图层加入了却找不到

                foreach (var part in product.CombinedParts.Where(it => it.IsDrawOn3D))
                {
                    Logger.GetLogger().Info(string.Format("Start Drawing Part:{0}/{1}/{2}/{3}", part.PartName, part.Material.Name, part.Length, part.Width));
                    Logger.GetLogger().Info(" - Bending:" + part.IsBend.ToString());
                    Logger.GetLogger().Info(" - IsMolding:" + part.IsMolding.ToString());
                    Logger.GetLogger().Info(" - Vdrillings:" + part.VDrillings.Count.ToString());
                    Logger.GetLogger().Info(" - Hdrillings:" + part.HDrillings.Count.ToString());
                    Logger.GetLogger().Info(" - Routings:" + part.Routings.Count.ToString());
                    Logger.GetLogger().Info(" - Profiles:" + part.Profiles.Count.ToString());

                    if (part.IsMolding)
                    {
                        (new PartMoldingDrawer(part, db, Path.Combine(Context.GetContext().MVDataContext.GetLatestRelease().MicrovellumData, "Graphics", "Moldings"))).Draw();
                    }
                    else if (part.IsBend)
                    {
                        (new PartBendDrawer(part, Point3d.Origin, db, offsetVector)).Draw();
                    }
                    else
                    {
                        string doorBlockPath = Path.Combine(library.MicrovellumData, "Graphics", "Blocks", part.PartName + ".dwg");
                        if (File.Exists(doorBlockPath))
                        {
                            (new BlockDrawer(part, db, doorBlockPath)).Draw();//放在后面 否则与profile等读取dwg文件的有冲突
                        }
                        else
                        {
                            (new PartDrawer(db, offsetVector)).Draw(part);
                        }
                    }
                }

                foreach (var hw in product.CombinedHardwares)
                {
                    (new HardwareDrawer(db, library)).Draw(hw);
                }

                HostApplicationServices.WorkingDatabase = oDb;//重要，否则会导致很多问题，比如图层加入了却找不到
                //另外先改回，在保存，否则会导致报错
                db.SaveAs(savePath, DwgVersion.Newest);
            }
        }
    }
}
