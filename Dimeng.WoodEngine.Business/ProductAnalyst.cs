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

        public ProductAnalyst()
        {

        }

        public IEnumerable<ModelError> Analysis(Product product, IWorkbookSet bookSet)
        {
            try
            {
                validateBookset(bookSet);
                this.workBookSet = bookSet;

                product.ClearData();

                var errors = getProductParts(product);

                

                return errors;
            }
            catch (Exception error)
            {
                throw new Exception("Error occures when analysising product:" + product.Description, error);
            }
        }

        private IEnumerable<ModelError> getProductParts(IProduct product)
        {
            IWorkbook book = workBookSet.Workbooks["L"];
            IRange cutPartCells = book.Worksheets[0].Cells;
            IRange subassembliesCells = book.Worksheets[2].Cells;
            IRange hardwareCells = book.Worksheets[1].Cells;

            List<ModelError> errors = new List<ModelError>();

            errors.AddRange(getParts(product, cutPartCells));//TODO：和下面的函数看起来是重复的

            return errors;
        }

        private IEnumerable<ModelError> getParts(IProduct product, IRange cutPartCells)
        {
            List<ModelError> errors = new List<ModelError>();

            for (int i = 0; i < cutPartCells.Rows.RowCount; i++)
            {
                if (string.IsNullOrEmpty(cutPartCells[i, 16].Text))
                {
                    break;//碰到空行中断
                }

                IRange partRow = cutPartCells[i, 0].EntireRow;

                List<Part> tempParts = new List<Part>();
                var partInitializer = new PartInitializer();
                errors.AddRange(partInitializer.GetPartsFromOneLine(partRow, product, tempParts, workBookSet, tempMaterials, tempEdgebandings));

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
