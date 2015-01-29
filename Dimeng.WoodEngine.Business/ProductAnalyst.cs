using Dimeng.LinkToMicrocad.Logging;
using Dimeng.WoodEngine.Entities;
using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Business
{
    public class ProductAnalyst
    {
        List<Material> tempMaterials = new List<Material>();
        List<EdgeBanding> tempEdgebandings = new List<EdgeBanding>();
        List<Hardware> tempHardwares = new List<Hardware>();
        private string SubassemblyLibraryPath;
        private IMVLibrary library;

        public ProductAnalyst(IMVLibrary library)
        {
            this.library = library;
        }

        public IEnumerable<ModelError> Analysis(Product product, IWorkbookSet bookSet)
        {
            Logger.GetLogger().Info("Analysising the product data....");

            try
            {
                validateBookset(bookSet);
                this.workBookSet = bookSet;

                product.ClearData();

                var errors = getIProductElements(product, workBookSet.Workbooks["L"]);

                return errors;
            }
            catch (Exception error)
            {
                throw new Exception("Error occures when analysising product:" + product.Description, error);
            }
        }

        private IEnumerable<ModelError> getIProductElements(IProduct product, IWorkbook book)
        {

            IRange cutPartCells = book.Worksheets[0].Cells;
            IRange subassembliesCells = book.Worksheets[2].Cells;
            IRange hardwareCells = book.Worksheets[1].Cells;

            List<ModelError> errors = new List<ModelError>();

            errors.AddRange(PartsLoader.GetParts(product, cutPartCells, workBookSet, tempMaterials, tempEdgebandings));
            //errors.AddRange(getParts(product, cutPartCells));
            errors.AddRange(getHardwares(product, hardwareCells));
            errors.AddRange(getSubassemblies(product, subassembliesCells));
            //TODO:Subassemblies
            //TODO:Hardwares

            return errors;
        }

        private IEnumerable<ModelError> getSubassemblies(IProduct product, IRange cells)
        {
            Logger.GetLogger().Info(string.Format("Getting subassemblies from product:{0}", product.Description));

            List<ModelError> errors = new List<ModelError>();

            for (int i = 0; i < cells.Cells.RowCount; i++)
            {
                var subRow = cells.Cells[i, 0].EntireRow;

                string subName = subRow[0, 16].Text;
                Logger.GetLogger().Debug(string.Format("Current row number:{0}/{1}/Q{2}", i + 1, subName, subRow[0, 17].Text));

                if (string.IsNullOrEmpty(subName.Trim()))
                {
                    Logger.GetLogger().Debug("Blank row and break the loop");
                    break;
                }

                List<Subassembly> subs = new List<Subassembly>();
                var subassemblyInitializer = new SubassemblyInitializer();
                var errorList = subassemblyInitializer.GetSubassembliesFromOneLine(subRow, product, subs,
                    workBookSet, tempMaterials, tempEdgebandings, tempHardwares, library);

                product.Subassemblies.AddRange(subs);

            }

            return errors;
        }

        private IEnumerable<ModelError> getHardwares(IProduct product, IRange cells)
        {
            Logger.GetLogger().Info(string.Format("Getting hardwares from product:{0}", product.Description));

            List<ModelError> errors = new List<ModelError>();

            //for (int i = 0; i < cells.Rows.RowCount; i++)
            //{
            //    Logger.GetLogger().Debug(string.Format("Current row number:{0}/{1}", i + 1, cells[i, 16].Text));

            //    if (string.IsNullOrEmpty(cells[i, 16].Text))
            //    {
            //        Logger.GetLogger().Debug("Blank row and break the loop.");
            //        break;
            //    }

            //    IRange row = cells[i, 0].EntireRow;

            //    List<Hardware> tempHardwares = new List<Hardware>();



            //    errors.AddRange(errorList);

            //    if (tempHardwares.Count > 0)
            //    {
            //        product.Hardwares.AddRange(tempHardwares);
            //    }
            //}

            return errors;
        }

        private void validateBookset(IWorkbookSet bookSet)
        {
            Logger.GetLogger().Debug("Validating the workbook set...");

            string[] keyIndexs = new string[] { "L", "G", "M", "E", "D", "H" };
            foreach (string i in keyIndexs)
            {
                if (bookSet.Workbooks[i] == null)
                {
                    throw new Exception(string.Format("The book {0} not found in product bookset.", i));
                }
            }
        }

        private IWorkbookSet workBookSet;
    }
}
