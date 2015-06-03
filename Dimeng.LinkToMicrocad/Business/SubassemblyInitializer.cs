using Dimeng.LinkToMicrocad.Logging;
using Dimeng.WoodEngine.Entities;
using Dimeng.WoodEngine.Entities.Checks;
using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Business
{
    public class SubassemblyInitializer
    {
        public List<ModelError> GetSubassembliesFromOneLine(IRange range, IProduct product,
            List<Subassembly> subs, IWorkbookSet books, List<Material> tempMaterials,
            List<EdgeBanding> tempEdgebandings, List<HardwareType> tempHardwares, IMVLibrary library)
        {
            List<ModelError> errors = new List<ModelError>();

            SubassemblyChecker check = new SubassemblyChecker(range, product.Description, errors);

            string subName = check.Name();
            int qty = check.Qty();
            if (qty <= 0)
            {
                return errors;
            }

            double width = check.Width();
            double height = check.Height();
            double depth = check.Depth();
            double xorigin = check.XOrigin();
            double yorigin = check.YOrigin();
            double zorigin = check.ZOrigin();
            double rotation = check.Rotation();
            string handle = check.Handle();
            int lineNumber = check.LineNumber();

            Logger.GetLogger().Debug(string.Format("Reading subassembly {0},W({1})H({2})D({3})", subName, width, height, depth));
            Logger.GetLogger().Debug(string.Format("-- XO({0})YO({1})ZO({2})Ro({3})", xorigin, yorigin, zorigin, rotation));

            Subassembly sub = new Subassembly(subName, qty, width, height, depth,
                xorigin, yorigin, zorigin, rotation, handle, lineNumber, product);
            subs.Add(sub);

            Project project = getSubassemblyProject(sub);
            string subFile = check.FileName(sub, library.Subassemblies, project.SubassembliesPath);

            IWorkbook bookSub = books.Workbooks.Open(subFile);
            bookSub.FullName = (sub.Parent is Product) ? "S" : "N";
            Logger.GetLogger().Debug("Subassembly full name is " + bookSub.FullName);

            try
            {
                Logger.GetLogger().Debug(string.Format("Fill subassembly workbook with W({0})H({1})D({2})",
                    sub.Width, sub.Height, sub.Depth));
                bookSub.Worksheets[3].Cells[0, 1].Value = sub.Width;
                bookSub.Worksheets[3].Cells[1, 1].Value = sub.Height;
                bookSub.Worksheets[3].Cells[2, 1].Value = sub.Depth;

                var partCells = bookSub.Worksheets[0].Cells;
                var subCells = bookSub.Worksheets[2].Cells;
                var hwrCells = bookSub.Worksheets[1].Cells;
                var machineCells = bookSub.Worksheets[4].Cells;

                errors.AddRange(PartsLoader.GetParts(sub, partCells, machineCells, books, tempMaterials, tempEdgebandings));
            }
            catch
            {
                throw;
            }
            finally
            {
                bookSub.Close();
            }

            return errors;
        }

        private Project getSubassemblyProject(Subassembly sub)
        {
            Project project;
            if (sub.Parent is Product)
            {
                project = (sub.Parent as Product).Project;
            }
            else
            {
                project = (sub.Parent.Parent as Product).Project;
            }
            return project;
        }
    }
}
