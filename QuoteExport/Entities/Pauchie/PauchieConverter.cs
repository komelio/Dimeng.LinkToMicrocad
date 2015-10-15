using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuoteExport.Entities
{
    public class PauchieConverter
    {
        Product product;
        private List<Subassembly> drawers = new List<Subassembly>();
        public PauchieConverter(Product product)
        {
            this.product = product;

            foreach (var sub in product.Subassemblies)
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

        public PauchieProduct GetPauchieProduct()
        {
            PauchieProduct pProduct = new PauchieProduct();
            pProduct.Description = product.Description;
            pProduct.Width = product.Width;
            pProduct.Height = product.Height;
            pProduct.Depth = product.Depth;
            pProduct.Qty = product.Qty;

            var combinedParts = new List<Part>();
            combinedParts.AddRange(product.Parts);
            var combinedHwrs = new List<Hardware>();
            combinedHwrs.AddRange(product.Hardwares);
            foreach (var sub in product.Subassemblies)
            {
                combinedParts.AddRange(sub.Parts);
                combinedHwrs.AddRange(sub.Hardwares);
                foreach (var s2 in sub.Subassemblies)
                {
                    combinedHwrs.AddRange(s2.Hardwares);
                    combinedParts.AddRange(s2.Parts);
                }
            }

            foreach (var p in combinedParts)
            {
                pProduct.Parts.Add(getPauchiePart(p));
            }

            
            foreach(var hw in combinedHwrs)
            {
                pProduct.Hardwares.Add(getPauchieHardware(hw));
            }

            return pProduct;
        }

        private PauchiePart getPauchiePart(Part part)
        {
            PauchiePart ppart = new PauchiePart(part);
            //ppart.Index = part.PartsCounter;
            ppart.Color = part.Material;
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

            string edgeColor1;
            ppart.EdgeSKU1 = getPartEdgeSKU(part, part.EBL1, part.EBL1Code, out edgeColor1);
            ppart.EdgeColor1 = edgeColor1;
            string edgeColor2;
            ppart.EdgeSKU2 = getPartEdgeSKU(part, part.EBL2, part.EBL2Code, out edgeColor2);
            ppart.EdgeColor2 = edgeColor2;
            string edgeColor3;
            ppart.EdgeSKU3 = getPartEdgeSKU(part, part.EBW1, part.EBW1Code, out edgeColor3);
            ppart.EdgeColor3 = edgeColor3;
            string edgeColor4;
            ppart.EdgeSKU4 = getPartEdgeSKU(part, part.EBW2, part.EBW2Code, out edgeColor4);
            ppart.EdgeColor4 = edgeColor4;

            ppart.MachiningArea = getPartMachiningArea(part);

            //if (part.HasFace5Machining())
            //{
            //    ppart.FileName = part.FileName;
            //}

            //if (part.HasFace6Machining())
            //{
            //    ppart.Face6FileName = part.Face6FileName;
            //}

            return ppart;
        }

        private string getPartSKU(Part part)
        {
            double length = part.CutLength;
            double width = part.CutWidth;

            string[] sheetSKUs = part.MaterialCode.Split(';');

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
            pHw.Description = hw.Name;
            pHw.SKU = hw.HardewareCode;
            pHw.Number = hw.Qty;
            pHw.Unit = "EACH";

            return pHw;
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

        private string getPartEdgeSKU(Part part, string edgeband, string edgecode, out string color)
        {
            var edge = edgeband;
            var cdes = edgecode == null ? new string[] { } : edgecode.Split(';');

            color = edgeband;
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

        private int getPartDrawerNumber(Part part)
        {
            if (!part.PartName.StartsWith("CT"))
            {
                return 0;
            }

            if (!(part.Parent is Subassembly))
            {
                return 0;
            }

            var s = part.Parent as Subassembly;

            var sub = drawers.Find(it =>
            {
                return it == s;
            });
            if (sub == null)
            {
                return 0;
            }

            return drawers.IndexOf(sub) + 1;
        }
    }
}
