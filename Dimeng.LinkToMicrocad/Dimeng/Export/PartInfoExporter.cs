using Dimeng.WoodEngine.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Dimeng.LinkToMicrocad
{
    public class PartInfoExporter
    {
        Product product;
        string path;
        public PartInfoExporter(Product product, string path)
        {
            this.product = product;
            this.path = path;
        }

        public void Export()
        {
            XDocument doc = new XDocument(new XDeclaration("1.0", "utf8", "no"));

            XElement xProduct = new XElement("Product",
                new XAttribute("Description", product.Description),
                new XAttribute("Comments", product.Comments),
                new XAttribute("Width", product.Width),
                new XAttribute("Height", product.Height),
                new XAttribute("Depth", product.Depth));

            XElement xParts = new XElement("Parts");
            xProduct.Add(xParts);

            foreach (var part in this.product.CombinedParts)
            {
                XElement xml = new XElement("Part");
                xml.Add(new XAttribute("PartName", part.PartName),
                    new XAttribute("Qty", part.Qty),
                    new XAttribute("Width", part.Width),
                    new XAttribute("Length", part.Length),
                    new XAttribute("Material", part.Material.Name),
                    new XAttribute("Thickness", part.Thickness),
                    new XAttribute("EBW1", part.EBW1.Name),
                    new XAttribute("EBW2", part.EBW2.Name),
                    new XAttribute("EBL1", part.EBL1.Name),
                    new XAttribute("EBL2", part.EBL2.Name),
                    new XAttribute("Comment", part.Comment),
                    new XAttribute("Comment2", part.Comment2),
                    new XAttribute("Comment3", part.Comment3)
                    );

                xParts.Add(xml);
            }

            doc.Add(xProduct);
            doc.Save(Path.Combine(this.path, "parts.xml"));
        }
    }
}
