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

            EdgeBanding ebw1 = check.EBW1(books.Workbooks["E"], tempEdgebandings);
            EdgeBanding ebw2 = check.EBW2(books.Workbooks["E"], tempEdgebandings);
            EdgeBanding ebl1 = check.EBL1(books.Workbooks["E"], tempEdgebandings);
            EdgeBanding ebl2 = check.EBL2(books.Workbooks["E"], tempEdgebandings);

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

                foreach (var d in origins)
                {
                    Part part = new Part(partName, 1, width, length, thick, material.Name,
                        ebw1.Name, ebw2.Name, ebl1.Name, ebl2.Name,
                        "", "", "",
                        basepoint, machinePoint.MP,
                        d[0], d[1], d[2], xRotation, yRotation, zRotation,
                        layname3d, layname2d,
                        product);
                    parts.Add(part);
                }
            }
            else
            {
                double xPos = check.XPosition();
                double yPos = check.YPosition();
                double zPos = check.ZPosition();
                Part part = new Part(partName, qty, width, length, thick, material.Name,
                    ebw1.Name, ebw2.Name, ebl1.Name, ebl2.Name,
                    "", "", "",
                    basepoint, machinePoint.MP,
                    xPos, yPos, zPos, xRotation, yRotation, zRotation,
                    layname3d, layname2d,
                    product);

                parts.Add(part);
            }

            return errors;
        }


    }
}
