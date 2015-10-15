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
        public PauchieExporter(List<PauchieProduct> products, string folderPath)
        {
            this.products = products;
            this.folderPath = folderPath;
        }

        public void Export()
        {
            foreach (var product in products)
            {
                string xmlPath = Path.Combine(folderPath, product.LineNumber + ".xml");
                XDocument doc = new XDocument(new XDeclaration("1.0", "utf8", "no"));

                XElement xml = new XElement("Product");
                xml.Add(new XAttribute("IsStd", "N"));
                xml.Add(new XAttribute("Qty", 1));
                xml.Add(new XAttribute("ShadArea", 0));
                xml.Add(new XAttribute("ItmTBName", string.Empty));
                xml.Add(new XAttribute("ItmTAName", string.Empty));
                xml.Add(new XAttribute("ItmSpec", 0));
                xml.Add(new XAttribute("ItmName", product.Description));
                xml.Add(new XAttribute("ItmID", string.Empty));
                //xml.Add(new XAttribute("OrderLine", product.LineNumber));
                xml.Add(new XAttribute("OrderNum", string.Empty));
                xml.Add(new XAttribute("LineNum", product.LineNumber));
                xml.Add(new XAttribute("BomID", string.Empty));

                XElement xParts = new XElement("Parts");
                xml.Add(xParts);
                foreach (var part in product.Parts)
                {
                    XElement x = new XElement("Part");
                    x.Add(new XAttribute("Qty", part.Qty));
                    x.Add(new XAttribute("ItmTBName", string.Empty));
                    x.Add(new XAttribute("ItmName", part.PartName));
                    x.Add(new XAttribute("ItmID", string.Empty));
                    x.Add(new XAttribute("LineNum", string.Empty));
                    x.Add(new XAttribute("Instruct", string.Empty));
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
            }
        }
    }
}
