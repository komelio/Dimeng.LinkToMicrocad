﻿using Dimeng.LinkToMicrocad.Logging;
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
            this.product = getPauchieProduct(prodakt);

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
        }

        public void Output()
        {
            string path = Path.Combine(prodakt.Project.JobPath, "Output", "BOM Output");
            Logger.GetLogger().Fatal(path);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            IWorkbook book = Factory.GetWorkbook();
            (new ExcelBuilder(this.product)).Build(book);
            book.SaveAs(Path.Combine(path, string.Format("{0}.xlsx", prodakt.Handle)), FileFormat.OpenXMLWorkbook);

            string pathToMachineCode = Path.Combine(prodakt.Project.JobPath, "Output", "Machinings");

        }

        private PauchieProduct getPauchieProduct(Product prodakt)
        {
            PauchieProduct pProudct = new PauchieProduct();
            pProudct.Description = prodakt.Description;
            pProudct.LineNumber = prodakt.Project.Products.IndexOf(prodakt) + 1;
            //pProudct.Qty = prodakt.Qty;
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

            foreach (var part in prodakt.CombinedParts)
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
            PauchiePart ppart = new PauchiePart();
            //ppart.Index = part.PartsCounter;
            //ppart.Color = 
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

            ppart.EdgeSKU = getPartEdgeSKU(part);

            ppart.MachiningArea = getPartMachiningArea(part);
            //ppart.FileName = part.FileName;

            return ppart;
        }
        private int getPartDrawerNumber(Part part)
        {
            if (!part.PartName.StartsWith("CT"))
            {
                return 0;
            }

            var sub = drawers.Find(it => it == part.Parent as Subassembly);
            if (sub == null)
            {
                return 0;
            }

            return drawers.IndexOf(sub) + 1;
        }

        private string getPartEdgeSKU(Part part)
        {
            var edges = new List<EdgeBanding>(new EdgeBanding[] { part.EBW1, part.EBW2, part.EBL1, part.EBL2 });
            edges = edges.Where(it => it.Name != EdgeBanding.Default().Name).ToList();

            if (edges.Count == 0)
            {
                return string.Empty;
            }

            if (edges.Select(it => it.Name).Distinct().ToList().Count > 1)
            {
                throw new Exception("封边数量大于1种");
            }

            var edge = edges[0];
            var cdes = edge.Code == null ? new string[] { } : edge.Code.Split(';');


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
