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

        public IEnumerable<ModelError> Analysis(Product product, IWorkbookSet bookSet)
        {
            Logger.GetLogger().Info("Analysising the product data....");

            try
            {
                bookSet.GetLock();

                validateBookset(bookSet);
                this.workBookSet = bookSet;

                product.ClearData();

                fillProductDimensionToLBook(product.Width, product.Height, product.Depth);

                IWorkbook book = workBookSet.Workbooks["L"];
                var errors = getIProductElements(product, book);

                bookSet.ReleaseLock();

                return errors;
            }
            catch (Exception error)
            {
                throw new Exception("Error occures when analysising product:" + product.Description, error);
            }
        }

        private void fillProductDimensionToLBook(double width, double height, double depth)
        {
            Logger.GetLogger().Debug("Fill the product workbook with product`s width/height/depth.");

            var sheet = this.workBookSet.Workbooks["L"].Worksheets["Prompts"];
            sheet.Cells[0, 1].Value = width;
            sheet.Cells[1, 1].Value = height;
            sheet.Cells[2, 1].Value = depth;

            Logger.GetLogger().Debug(string.Format("W({0})H({1})D({2}).", width, height, depth));
        }

        private IEnumerable<ModelError> getIProductElements(IProduct product, IWorkbook book)
        {

            IRange cutPartCells = book.Worksheets[0].Cells;
            IRange subassembliesCells = book.Worksheets[2].Cells;
            IRange hardwareCells = book.Worksheets[1].Cells;

            List<ModelError> errors = new List<ModelError>();

            errors.AddRange(getParts(product, cutPartCells));//TODO：和下面的函数看起来是重复的
            //TODO:Subassemblies
            //TODO:Hardwares

            return errors;
        }

        private IEnumerable<ModelError> getParts(IProduct product, IRange cutPartCells)
        {
            Logger.GetLogger().Info(string.Format("Getting parts from product:{0}", product.Description));

            List<ModelError> errors = new List<ModelError>();

            for (int i = 0; i < cutPartCells.Rows.RowCount; i++)
            {
                Logger.GetLogger().Debug(string.Format("Current row number:{0}/{1}", i + 1, cutPartCells[i, 16].Text));

                if (string.IsNullOrEmpty(cutPartCells[i, 16].Text))
                {
                    Logger.GetLogger().Debug("Blank row and break the loop.");
                    break;
                }

                IRange partRow = cutPartCells[i, 0].EntireRow;

                List<Part> tempParts = new List<Part>();
                var partInitializer = new PartInitializer();
                var errorList = partInitializer.GetPartsFromOneLine(partRow, product, tempParts,
                                    workBookSet, tempMaterials, tempEdgebandings);

                errors.AddRange(errorList);

                if (tempParts.Count > 0)
                {
                    product.Parts.AddRange(tempParts);
                }
            }

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
