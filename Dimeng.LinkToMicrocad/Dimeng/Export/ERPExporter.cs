using Dimeng.LinkToMicrocad.Logging;
using Dimeng.WoodEngine.Entities;
using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dimeng.LinkToMicrocad
{
    public class ERPExporter
    {
        PauchieProduct product;
        Product prodakt;
        List<Subassembly> drawers = new List<Subassembly>();

        public ERPExporter(Product prodakt)
        {
            this.prodakt = prodakt;
            foreach (var sub in prodakt.Subassemblies)
            {
                drawers.Add(sub);
                foreach (var sub2 in sub.Subassemblies)
                {
                    drawers.Add(sub2);
                }
            }
            drawers = drawers.Where(it => it.Parts.Any(i => i.PartName.StartsWith("CT")))
                             .ToList();
            Logger.GetLogger().Debug(string.Format("drawer`s count :{0}", drawers.Count));

            this.product = getPauchieProduct(prodakt);//放在最后，因为涉及抽屉号码计算，需要先统计数量
        }

        public void Output()
        {
            string path = Path.Combine(prodakt.Project.JobPath, "Output", prodakt.Handle);
            Logger.GetLogger().Debug(path);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            else
            {
                //删除原有的所有内容
                var files = Directory.GetFiles(path);
                foreach (var f in files)
                {
                    File.Delete(f);
                }
            }

            IWorkbook book = Factory.GetWorkbook();
            (new ExcelBuilder(this.product)).Build(book);
            book.SaveAs(Path.Combine(path, string.Format("{0}.xlsx", prodakt.Handle)), FileFormat.OpenXMLWorkbook);

            string pathToMachineCode = Path.Combine(path, "Machinings");
            if (!Directory.Exists(pathToMachineCode))
            {
                Directory.CreateDirectory(pathToMachineCode);
            }

            //先删除掉原有的csv文件
            foreach(var f in Directory.GetFiles(pathToMachineCode))
            {
                File.Delete(f);
            }

            foreach (var part in this.prodakt.CombinedParts)
            {
                MachiningExporter me = new MachiningExporter(part, pathToMachineCode);
                me.Export();
            }
        }

        private PauchieProduct getPauchieProduct(Product prodakt)
        {
            PauchieProduct pProudct = new PauchieProduct();
            pProudct.Description = prodakt.Description;
            pProudct.LineNumber = prodakt.Project.Products.IndexOf(prodakt) + 1;
            pProudct.Qty = 1;
            //pProudct.OrderNumber = prodakt.Project.ProjectInfo.JobName;

            //foreach (var hw in prodakt.CombinedHardwares)
            //{
            //    var pHw = pProudct.Hardwares.Find(it => it.Description.ToLower() == hw.Name.ToLower() && it.SKU.ToLower() == hw..HardwareCode);
            //    if (pHw != null)
            //    {
            //        pHw.Number += hw.Qty;
            //    }
            //    else
            //    {
            //        pProudct.Hardwares.Add(getPauchieHardware(hw));
            //    }
            //}

            foreach (var part in prodakt.CombinedParts.Where(it => !it.Material.IsFake))
            {
                pProudct.Parts.Add(getPauchiePart(part));

                //getTokenHardwares(part, pProudct.Hardwares);
            }

            return pProudct;
        }

        private void getTokenHardwares(Part part, List<PauchieHardware> list)
        {
            throw new NotImplementedException();
        }

        private PauchiePart getPauchiePart(Part part)
        {
            PauchiePart ppart = new PauchiePart(part);
            //ppart.Index = part.PartsCounter;
            ppart.Color = part.Material.Name;
            ppart.PartName = part.PartName;
            ppart.Category = part.PartName;

            ppart.SKU = getPartSKU(part);

            ppart.Qty = part.Qty;
            ppart.Model = ((part.Comment3 == null ? string.Empty : part.Comment3).IndexOf("y") > -1) ? "Y" : "N";

            ppart.CutLength = part.CutLength;
            ppart.CutWidth = part.CutWidth;
            ppart.Length = part.Length;
            ppart.Width = part.Width;
            ppart.CutThickness = part.Thickness;
            ppart.Thickness = part.Thickness;
            ppart.CutSquare = Math.Round(part.CutLength * part.CutWidth / 1000000, 2);
            ppart.Square = Math.Round(part.Length * part.Width / 1000000, 2);
            ppart.DrawerNumber = getPartDrawerNumber(part);

            string edgeColor;
            ppart.EdgeSKU = getPartEdgeSKU(part, out edgeColor);
            ppart.EdgeColor = edgeColor;

            ppart.MachiningArea = getPartMachiningArea(part);

            if (part.HasFace5Machining())
            {
                ppart.FileName = part.FileName;
            }

            if (part.HasFace6Machining())
            {
                ppart.Face6FileName = part.Face6FileName;
            }

            return ppart;
        }
        private int getPartDrawerNumber(Part part)
        {
            if (!part.PartName.StartsWith("CT"))
            {
                Logger.GetLogger().Debug(string.Format("{0} is not a CT part", part.PartName));
                return 0;
            }

            if (!(part.Parent is Subassembly))
            {
                Logger.GetLogger().Debug(string.Format("{0}`s parent is not a subassembly", part.PartName));
                return 0;
            }

            var s = part.Parent as Subassembly;

            var sub = drawers.Find(it =>
                {
                    return it == s;
                });
            if (sub == null)
            {
                Logger.GetLogger().Debug(string.Format("{0}`s parent not found in drawers list", part.PartName));
                return 0;
            }

            return drawers.IndexOf(sub) + 1;
        }

        private string getPartEdgeSKU(Part part, out string color)
        {
            var edges = new List<EdgeBanding>(new EdgeBanding[] { part.EBW1, part.EBW2, part.EBL1, part.EBL2 });
            edges = edges.Where(it => it.Name != EdgeBanding.Default().Name).ToList();

            if (edges.Count == 0)
            {
                color = string.Empty;
                return string.Empty;
            }

            if (edges.Select(it => it.Name).Distinct().ToList().Count > 1)
            {
                throw new Exception("封边数量大于1种");
            }

            var edge = edges[0];
            var cdes = edge.Code == null ? new string[] { } : edge.Code.Split(';');

            color = edge.Name;
            if (Math.Abs(part.Thickness - 18) < 1)
            {
                if (cdes.Length == 0)
                {
                    throw new Exception("封边材料SKU不对");
                }

                return cdes[0];
            }
            else if (Math.Abs(part.Thickness - 25) < 1)
            {
                if (cdes.Length <= 1)
                {
                    throw new Exception("封边材料SKU不对");
                }

                return cdes[1];
            }
            else if (Math.Abs(part.Thickness - 36) < 1)
            {
                if (cdes.Length <= 2)
                {
                    throw new Exception("封边材料SKU不对");
                }

                return cdes[2];
            }
            else if (Math.Abs(part.Thickness - 50) < 1)
            {
                if (cdes.Length <= 3)
                {
                    throw new Exception("封边材料SKU不对");
                }

                return cdes[3];
            }

            return "NotFound!";
        }

        private int getPartMachiningArea(Part part)
        {
            if (part.PartName.StartsWith("CT"))//抽屉
            {
                return 3;
            }
            else if (part.Thickness < 10)//背板
            {
                return 2;
            }
            else//普通板件
            {
                return 1;
            }
        }
        private string getPartSKU(Part part)
        {
            double length = part.CutLength;
            double width = part.CutWidth;

            string[] sheetSKUs = part.Material.Code.Split(';');

            if ((length < 2430 && width <= 1210) || (length <= 1210 && length <= 2430))
            {
                return sheetSKUs[0];
            }
            else
            {
                if (sheetSKUs.Length <= 1)
                {
                    throw new Exception("材料不包含多个sku：" + part.Material);
                }
                return sheetSKUs[1];
            }
        }

        private PauchieHardware getPauchieHardware(Hardware hw)
        {
            PauchieHardware pHw = new PauchieHardware();
            //pHw.Description = hw.Name;
            //pHw.SKU = hw.Code == null ? string.Empty : hw.Code;
            //pHw.Number = hw.Qty;
            //pHw.Unit = "EACH";


            return pHw;
        }
    }
}
