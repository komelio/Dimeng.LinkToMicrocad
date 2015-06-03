using Dimeng.LinkToMicrocad.Logging;
using Dimeng.WoodEngine.Entities;
using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Business
{
    public class PartsLoader
    {
        public static IEnumerable<ModelError> GetParts(IProduct product, IRange cutPartCells, IRange machineCells,
            IWorkbookSet workBookSet, List<Material> tempMaterials, List<EdgeBanding> tempEdgebandings)
        {
            Logger.GetLogger().Info(string.Format("Getting parts from product:{0}", product.Description));

            List<ModelError> errors = new List<ModelError>();

            for (int i = 0; i < cutPartCells.Rows.RowCount; i++)
            {
                Logger.GetLogger().Debug(string.Format("Current row number:{0}/{1}/Q{2}", i + 1, cutPartCells[i, 16].Text, cutPartCells[i, 17].Text));

                if (string.IsNullOrEmpty(cutPartCells[i, 16].Text.Trim()))
                {
                    Logger.GetLogger().Debug("Blank row and break the loop.");
                    break;
                }

                IRange partRow = cutPartCells[i, 0].EntireRow;
                IRange machineColumn = machineCells[0, i].EntireColumn;

                List<Part> tempParts = new List<Part>();
                var partInitializer = new PartInitializer();
                var errorList = partInitializer.GetPartsFromOneLine(partRow, machineColumn, product, tempParts,
                                    workBookSet, tempMaterials, tempEdgebandings);

                errors.AddRange(errorList);

                if (tempParts.Count > 0)
                {
                    product.Parts.AddRange(tempParts);
                }
            }

            return errors;
        }
    }
}
