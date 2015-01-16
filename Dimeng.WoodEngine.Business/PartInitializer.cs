using Dimeng.WoodEngine.Entities;
using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Business
{
    public class PartInitializer
    {
        public List<ModelError> GetPartsFromOneLine(IRange partRange, IProduct product, IEnumerable<Part> parts, IWorkbookSet books, List<Material> tempMaterials, List<EdgeBanding> tempEdgebandings)
        {
            List<ModelError> errors = new List<ModelError>();

            PartChecker check = new PartChecker(partRange, errors);

            string partName = check.PartName();
            int qty = check.Qty();
            if (qty <= 0)
            {
                return errors;
            }

            double width = check.Width();
            double length = check.Length();

            Material material;
            double thick = check.Thick(books.Workbooks["M"], out material, tempMaterials, errors);

            return errors;
        }


    }
}
