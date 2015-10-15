using Dimeng.LinkToMicrocad.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Dimeng.LinkToMicrocad.Dimeng.Export
{
    public class PushXMLExporter
    {
        PauchieProduct product;
        string filepath;
        public PushXMLExporter(PauchieProduct product, string filename)
        {
            this.filepath = filename;
            this.product = product;
        }

        public void Build()
        {
            Logger.GetLogger().Debug("Part total number:" + product.Parts.Count.ToString());
            Logger.GetLogger().Debug("Hardware total number:" + product.Hardwares.Count.ToString());

            XDocument xmlDoc = new XDocument(new XDeclaration("1.0", "utf-8", "no"));

            XElement xml = new XElement("Product",
                new XAttribute("IsStd", "N"),
                new XAttribute("Qty", 1),
                new XAttribute("ShadArea", 0),
                new XAttribute("ItmTBName", ""),
                new XAttribute("ItmTAName", ""),
                new XAttribute("ItmSpec", ""),
                new XAttribute("ItmName", product.Description),
                new XAttribute("ItmID", ""),
                new XAttribute("OrderLine", ""),
                new XAttribute("OrderNum", ""),
                new XAttribute("LineNum", ""),
                new XAttribute("BomID", "")
                );

            XElement xmlParts = new XElement("Parts");
            foreach (var part in product.Parts)
            {
                xmlParts.Add(new XElement("Part",
                    new XAttribute("Qty", part.Qty),
                    new XAttribute("ItmTBName", ""),
                    new XAttribute("ItmName", part.PartName),
                    new XAttribute("ItmID", ""),
                    new XAttribute("LineNum", ""),
                    new XAttribute("Instruct", part.FileName ?? string.Empty),
                    new XAttribute("Drawer", part.DrawerNumber),
                    new XAttribute("Station", part.MachiningArea),
                    new XAttribute("Width", part.Width),
                    new XAttribute("Length", part.Length),
                    new XAttribute("IsSculpt", "false"),
                    new XAttribute("EdgeItmID4", part.EdgeSKU ?? string.Empty),
                    new XAttribute("EdgeItmID3", part.EdgeSKU ?? string.Empty),
                    new XAttribute("EdgeItmID2", part.EdgeSKU ?? string.Empty),
                    new XAttribute("EdgeItmID1", part.EdgeSKU ?? string.Empty),
                    new XAttribute("MetItmID", part.SKU ?? string.Empty)));
            }

            XElement xmlHwrs = new XElement("Hardwares");
            foreach (var hwr in product.Hardwares)
            {
                xmlHwrs.Add(new XElement("HWR",
                    new XAttribute("Qty", hwr.Number),
                    new XAttribute("ItmName", hwr.Description),
                    new XAttribute("ItmID", hwr.SKU),
                    new XAttribute("LineNum", "")));
            }

            xml.Add(xmlParts);
            xml.Add(xmlHwrs);

            xmlDoc.Add(xml);
            xmlDoc.Save(filepath);
        }
    }
}
