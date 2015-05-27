using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.LinkToMicrocad.Drawing
{
    public class LayerHelper
    {
        public static string SetLayer(Database db, string layerName)
        {
            if (string.IsNullOrEmpty(layerName.Trim()))
            {
                throw new Exception("layer name could not be empty");
            }

            using (Transaction tran = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = (LayerTable)tran.GetObject(db.LayerTableId, OpenMode.ForWrite);
                if (!lt.Has(layerName))
                {
                    LayerTableRecord ltr = new LayerTableRecord();
                    ltr.Name = layerName;

                    lt.Add(ltr);
                    tran.AddNewlyCreatedDBObject(ltr, true);
                    tran.Commit();
                }
            }

            return layerName;
        }
    }
}
