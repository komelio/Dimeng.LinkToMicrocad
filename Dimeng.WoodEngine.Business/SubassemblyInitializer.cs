using Dimeng.LinkToMicrocad.Logging;
using Dimeng.WoodEngine.Entities;
using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Business
{
    public class SubassemblyInitializer
    {
        public List<ModelError> GetSubassembliesFromOneLine(IRange partRange, IProduct product,
            List<Subassembly> subs, IWorkbookSet books, List<Material> tempMaterials,
            List<EdgeBanding> tempEdgebandings, List<Hardware> tempHardwares)
        {
            List<ModelError> errors = new List<ModelError>();

            SubassemblyChecker check = new SubassemblyChecker(partRange, product.Description, errors);

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
            return errors;
        }
    }
}
