using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace QuoteExport.Entities
{
    public class PauchieExporter
    {
        List<PauchieProduct> products;
        string folderPath;
        public List<string> Files { get; private set; }
        public PauchieExporter(List<PauchieProduct> products, string folderPath)
        {
            this.products = products;
            this.folderPath = folderPath;
            this.Files = new List<string>();
        }

        public void Export()
        {
            outputListXML();

            foreach (var product in products)
            {
                string xmlPath = Path.Combine(folderPath, product.BomId + ".xml");
                XDocument doc = new XDocument(new XDeclaration("1.0", "utf8", "no"));

                XElement xml = new XElement("Product");
                xml.Add(new XAttribute("IsStd", "N"));
                xml.Add(new XAttribute("Qty", 1));
                xml.Add(new XAttribute("ShadArea", System.Math.Round(product.Width * product.Height / 1000000, 2)));
                xml.Add(new XAttribute("ItmTBName", string.Empty));//todo
                xml.Add(new XAttribute("ItmTAName", product.Category));
                xml.Add(new XAttribute("ItmSpec", string.Format("{0}*{1}*{2}", product.Width, product.Height, product.Depth)));
                xml.Add(new XAttribute("ItmName", product.Description));
                xml.Add(new XAttribute("ItmID", product.ItmId));
                //xml.Add(new XAttribute("OrderLine", product.LineNumber));
                xml.Add(new XAttribute("OrderNum", string.Empty));//from erp
                xml.Add(new XAttribute("OrderLine", string.Empty));//from erp
                xml.Add(new XAttribute("LineNum", product.LineNumber));
                xml.Add(new XAttribute("BomID", product.BomId));

                XElement xParts = new XElement("Parts");
                xml.Add(xParts);
                int partLineNum = 0;
                foreach (var part in product.Parts)
                {
                    partLineNum++;

                    XElement x = new XElement("Part");
                    x.Add(new XAttribute("Qty", part.Qty));
                    x.Add(new XAttribute("ItmTBName", part.Color));
                    x.Add(new XAttribute("ItmName", part.PartName));
                    x.Add(new XAttribute("ItmID", part.PartName));
                    x.Add(new XAttribute("LineNum", partLineNum));
                    x.Add(new XAttribute("Instruct", part.FileName));
                    x.Add(new XAttribute("Drawer", part.DrawerNumber));
                    x.Add(new XAttribute("Station", part.MachiningArea));
                    x.Add(new XAttribute("Width", part.Width));
                    x.Add(new XAttribute("Length", part.Length));
                    x.Add(new XAttribute("IsSculpt", "N"));
                    x.Add(new XAttribute("EdgeItmID4", part.EdgeSKU4));
                    x.Add(new XAttribute("EdgeItmID3", part.EdgeSKU3));
                    x.Add(new XAttribute("EdgeItmID2", part.EdgeSKU2));
                    x.Add(new XAttribute("EdgeItmID1", part.EdgeSKU1));
                    x.Add(new XAttribute("MetItmID", part.SKU));
                    xParts.Add(x);
                }

                XElement xHwrs = new XElement("Hardwares");
                xml.Add(xHwrs);
                foreach (var hw in product.Hardwares)
                {
                    XElement x = new XElement("HWR");
                    x.Add(new XAttribute("Qty", hw.Number));
                    x.Add(new XAttribute("ItmName", hw.Description));
                    x.Add(new XAttribute("ItmID", hw.SKU));
                    x.Add(new XAttribute("LineNum", product.Hardwares.IndexOf(hw) + 1));
                }

                doc.Add(xml);
                doc.Save(xmlPath);
                Files.Add(xmlPath);
            }
        }

        private void outputListXML()
        {
            string path = Path.Combine(folderPath, "bom.xml");
            XDocument doc = new XDocument(new XDeclaration("1.0", "utf8", "no"));

            XElement xml = new XElement("Products");
            foreach (var p in products)
            {
                xml.Add(new XElement("Product",
                    new XAttribute("BomID", p.BomId),
                    new XAttribute("LineNum", p.LineNumber),
                    new XAttribute("OrderNum", p.OrderNumber),
                    new XAttribute("OrderLine", 0),
                    new XAttribute("ItmID", p.ItmId),
                    new XAttribute("ItmName", p.Description),
                    new XAttribute("ItmTAName", "柜体"),
                    new XAttribute("ItmTBName", string.Empty),
                    new XAttribute("IsStd", "N")));
            }

            doc.Add(xml);
            doc.Save(path);
            Files.Add(path);
        }
    }
}
