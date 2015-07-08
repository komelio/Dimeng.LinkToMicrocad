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
    public class HardwareLoader
    {
        public static IEnumerable<ModelError> GetHardwares(IProduct product, IRange hardwareCells,
            IWorkbookSet workBookSet, List<HardwareType> hardwareTypes)
        {
            Logger.GetLogger().Info(string.Format("Getting hardwares from product:{0}", product.Description));
            List<ModelError> errors = new List<ModelError>();

            for (int i = 0; i < hardwareCells.Rows.RowCount; i++)
            {
                Logger.GetLogger().Debug(string.Format("Current row number:{0}/{1}/Q{2}", i + 1, hardwareCells[i, 16].Text, hardwareCells[i, 17].Text));

                if (string.IsNullOrEmpty(hardwareCells[i, 16].Text.Trim()))
                {
                    Logger.GetLogger().Debug("Blank row and break the loop.");
                    break;
                }


                IRange partRow = hardwareCells[i, 0].EntireRow;
                HardwareChecker check = new HardwareChecker(partRow, getLocation(product), errors);
                List<Hardware> tempHardwares = new List<Hardware>();

                int qty = check.Qty();
                if (qty <= 0)
                {
                    continue;
                }

                Hardware hwr = new Hardware();
                hwr.Name = check.HardwareName();
                hwr.Qty = qty;
                hwr.Width = check.Width();
                hwr.Height = check.Height();
                hwr.Depth = check.Depth();
                hwr.HardwareType = check.Hardwaretype(workBookSet.Workbooks["H"], hardwareTypes);
                bool isHaveAssociateRotation;
                double tAssociateAngle;
                hwr.AssociatedRotation = check.AssociateRotation(out isHaveAssociateRotation, out tAssociateAngle);
                hwr.IsHaveAssocaitedRotation = isHaveAssociateRotation;
                hwr.TAssociatedRotation = tAssociateAngle;

                bool isEQ = check.IsEQ();
                if (isEQ)
                {
                    List<double[]> origins = check.EQPositions(qty, 0, product.Width);

                    if (errors.Count > 0)
                    {
                        Logger.GetLogger().Warn("Hardware got errors !");
                        //return errors; //有问题的板件还是画
                    }

                    foreach (var d in origins)
                    {
                        Hardware cloneHwr = new Hardware();
                        cloneHwr.Name = hwr.Name;
                        cloneHwr.Qty = 1;
                        cloneHwr.Width = hwr.Width;
                        cloneHwr.Height = hwr.Height;
                        cloneHwr.Depth = hwr.Depth;
                        cloneHwr.HardwareType = hwr.HardwareType;
                        cloneHwr.AssociatedRotation = hwr.AssociatedRotation;
                        cloneHwr.IsHaveAssocaitedRotation = hwr.IsHaveAssocaitedRotation;
                        cloneHwr.TAssociatedRotation = hwr.TAssociatedRotation;
                        cloneHwr.XOrigin = d[0];
                        cloneHwr.YOrigin = d[1];
                        cloneHwr.ZOrigin = d[2];
                        cloneHwr.TXOrigin = d[0];
                        cloneHwr.TYOrigin = d[1];
                        cloneHwr.TZOrigin = d[2];

                        tempHardwares.Add(cloneHwr);
                        Logger.GetLogger().Debug(string.Format("Add hardware {0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}",
                            cloneHwr.Name, cloneHwr.Qty, cloneHwr.Width, cloneHwr.Height, cloneHwr.Depth, cloneHwr.XOrigin, cloneHwr.YOrigin, cloneHwr.ZOrigin, cloneHwr.AssociatedRotation));
                    }
                }
                else
                {
                    double xPos = check.XPosition();
                    double yPos = check.YPosition();
                    double zPos = check.ZPosition();

                    if (errors.Count > 0)
                    {
                        Logger.GetLogger().Info("Hardware got errors !");
                        //return errors; //有问题的板件还是画
                    }

                    hwr.XOrigin = xPos;
                    hwr.YOrigin = yPos;
                    hwr.ZOrigin = zPos;
                    hwr.TXOrigin = xPos;
                    hwr.TYOrigin = yPos;
                    hwr.TZOrigin = zPos;
                    tempHardwares.Add(hwr);
                    Logger.GetLogger().Debug(string.Format("Add hardware {0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}",
                        hwr.Name, hwr.Qty, hwr.Width, hwr.Height, hwr.Depth, hwr.XOrigin, hwr.YOrigin, hwr.ZOrigin, hwr.AssociatedRotation));
                }

                if (tempHardwares.Count > 0)
                {
                    product.Hardwares.AddRange(tempHardwares);
                }
            }

            return errors;
        }
        private static string getLocation(IProduct product)
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
