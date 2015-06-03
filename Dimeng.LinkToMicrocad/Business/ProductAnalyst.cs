using Autodesk.AutoCAD.Geometry;
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
        List<HardwareType> tempHardwares = new List<HardwareType>();

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

                combinedPartsAndHardwaresAndGetMachining(product);

                return errors;
            }
            catch (Exception error)
            {
                throw new Exception("Error occures when analysising product:" + product.Description, error);
            }
        }

        private void combinedPartsAndHardwaresAndGetMachining(Product product)
        {
            Logger.GetLogger().Info("Get product combined collections...");

            product.Parts.ForEach(it => it.CalculateLocationInfo(Point3d.Origin, 0));

            product.CombinedParts.AddRange(product.Parts);
            product.CombinedHardwares.AddRange(product.Hardwares);

            foreach (var sub in product.Subassemblies)
            {
                Logger.GetLogger().Debug(
                    string.Format("Sub:{0}/{1}/{2}/{3}/{4}", sub.Description, sub.Width, sub.Height, sub.Depth, sub.Rotation));

                sub.Parts.ForEach(it => it.CalculateLocationInfo(
                    new Point3d(sub.XOrigin, sub.YOrigin, sub.ZOrigin), sub.Rotation));
                product.CombinedHardwares.AddRange(sub.Hardwares);
                product.CombinedParts.AddRange(sub.Parts);

                foreach (var sub2 in sub.Subassemblies)
                {
                    sub2.Parts.ForEach(it => it.CalculateLocationInfo(
                        new Point3d(sub.XOrigin + sub2.XOrigin, sub.YOrigin + sub2.YOrigin, sub.ZOrigin + sub2.ZOrigin), sub.Rotation + sub2.Rotation));

                    product.CombinedHardwares.AddRange(sub2.Hardwares);
                    product.CombinedParts.AddRange(sub2.Parts);
                }
            }

            product.CombinedParts.ForEach(p => p.MachineTokens.ForEach(m => m.ToMachining(1, null)));
        }

        private IEnumerable<ModelError> getIProductElements(IProduct product, IWorkbook book)
        {

            IRange cutPartCells = book.Worksheets[0].Cells;
            IRange subassembliesCells = book.Worksheets[2].Cells;
            IRange hardwareCells = book.Worksheets[1].Cells;
            IRange machineCells = book.Worksheets[4].Cells;

            List<ModelError> errors = new List<ModelError>();

            errors.AddRange(PartsLoader.GetParts(product, cutPartCells, machineCells, workBookSet, tempMaterials, tempEdgebandings));
            //errors.AddRange(getParts(product, cutPartCells));
            errors.AddRange(HardwareLoader.GetHardwares(product, hardwareCells, workBookSet, tempHardwares));
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
