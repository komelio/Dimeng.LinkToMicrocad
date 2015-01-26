using Dimeng.LinkToMicrocad.Logging;
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
        public List<ModelError> GetPartsFromOneLine(IRange partRange, IProduct product, List<Part> parts, IWorkbookSet books, List<Material> tempMaterials, List<EdgeBanding> tempEdgebandings)
        {
            List<ModelError> errors = new List<ModelError>();

            PartChecker check = new PartChecker(partRange, getLocation(product), errors);

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

            EdgeBanding ebw1 = check.EBW1(books.Workbooks["E"], tempEdgebandings);
            EdgeBanding ebw2 = check.EBW2(books.Workbooks["E"], tempEdgebandings);
            EdgeBanding ebl1 = check.EBL1(books.Workbooks["E"], tempEdgebandings);
            EdgeBanding ebl2 = check.EBL2(books.Workbooks["E"], tempEdgebandings);

            string[] comments = partRange[0, 26].Text.Split('|');//Column26
            string comment = comments[0];
            string comment2 = (comments.Length > 1) ? comments[1] : string.Empty;
            string comment3 = (comments.Length > 2) ? comments[2] : string.Empty;

            int basepoint = check.BasePoint();
            MachinePoint machinePoint = check.MachinePoint();

            double xRotation = check.XRotation();
            double yRotation = check.YRotation();
            double zRotation = check.ZRotation();

            bool isDraw3d;
            string layname3d = check.Layname3d(out isDraw3d);
            string layname2d = partRange[0, 36].Text;

            if (width <= 0 || length <= 0 || thick <= 0)
            {
                return errors;
            }

            check.IsMaterialFitPartLengthAndWidth(length, width, material, errors);

            //TODO:tokens

            bool isEQPart = check.IsEQPart();
            if (isEQPart)
            {
                List<double[]> origins = check.Positions(qty, thick, product.Width);

                if (errors.Count > 0)
                {
                    return errors;
                }

                foreach (var d in origins)
                {
                    Part part = new Part(partName, 1, width, length, thick, material,
                        ebw1, ebw2, ebl1, ebl2,
                        comment, comment2, comment3,
                        basepoint, machinePoint,
                        d[0], d[1], d[2], xRotation, yRotation, zRotation,
                        layname3d, layname2d,
                        product);
                    parts.Add(part);
                    Logger.GetLogger().Debug(string.Format("{0}/{1}/{2}/{3}", partName, 1, width, length));
                }
            }
            else
            {
                double xPos = check.XPosition();
                double yPos = check.YPosition();
                double zPos = check.ZPosition();

                if (errors.Count > 0)
                {
                    return errors;
                }

                Part part = new Part(partName, qty, width, length, thick, material,
                    ebw1, ebw2, ebl1, ebl2,
                    comment, comment2, comment3,
                    basepoint, machinePoint,
                    xPos, yPos, zPos, xRotation, yRotation, zRotation,
                    layname3d, layname2d,
                    product);

                parts.Add(part);
                Logger.GetLogger().Debug(string.Format("{0}/{1}/{2}/{3}", partName, qty, width, length));
            }

            return errors;
        }

        private string getLocation(IProduct product)
        {
            if (product is Product)
            {
                var _product = product as Product;
                return string.Format("{0} {1}", _product.Handle, _product.Description);
            }

            if (product is Subassembly)
            {
                var _sub = product as Subassembly;
                return string.Format("{0}", _sub.Description);//TODO
            }

            throw new Exception("Unknown type of product");
        }


    }
}
