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

            Logger.GetLogger().Debug("Drawing product elements....");
            Logger.GetLogger().Debug("Parts:" + product.Parts.Count.ToString());

            using (Database db = new Database())
            {
                Database oDb = HostApplicationServices.WorkingDatabase;
                HostApplicationServices.WorkingDatabase = db;//重要，否则会导致很多问题，比如图层加入了却找不到

                foreach (var part in product.Parts)
                {
                    (new PartDrawer(db)).Draw(part);
                }

                foreach (var sub in product.Subassemblies)
                {
                    Logger.GetLogger().Debug(string.Format("Drawing subassembly {0} elements....", sub.Description));
                    Logger.GetLogger().Debug("Parts:" + sub.Parts.Count.ToString());
                    foreach (var part in sub.Parts)
                    {
                        (new PartDrawer(db)).Draw(part);
                    }
                }

                HostApplicationServices.WorkingDatabase = oDb;//重要，否则会导致很多问题，比如图层加入了却找不到
                //另外先改回，在保存，否则会导致报错
                db.SaveAs(savePath, DwgVersion.Newest);
            }
        }

        private static void prepareData(Product product)
        {
            product.Parts.ForEach(it => it.CalculateLocationInfo(Point3d.Origin, 0));

            foreach (var sub in product.Subassemblies)
            {
                sub.Parts.ForEach(it => it.CalculateLocationInfo(
                    new Point3d(sub.XOrigin, sub.YOrigin, sub.ZOrigin), sub.Rotation));
            }

            product.Parts.ForEach(p => p.MachineTokens.ForEach(m => m.ToMachining(0.1, null)));
        }
    }
}
