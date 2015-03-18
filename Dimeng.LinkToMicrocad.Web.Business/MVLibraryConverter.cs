using Dimeng.LinkToMicrocad.Web.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Dimeng.LinkToMicrocad.Web.Business
{
    public class MVLibraryConverter
    {
        private IProductRepository repository;
        private string saveXMLPath;

        public MVLibraryConverter(IProductRepository repo, string savePath)
        {
            this.repository = repo;
            this.saveXMLPath = Path.Combine(savePath, "Dms.xml");
        }

        public void ConvertToXML()
        {
            if (File.Exists(saveXMLPath))
            { File.Delete(saveXMLPath); }

            List<string> categoreis = repository.Products.Select(x => x.Category)
                                                         .Distinct()
                                                         .ToList();

            XDocument doc = new XDocument();
            XElement root = new XElement("Root");
            XElement dimengCategory = new XElement("Category",
                                                new XAttribute("K", "kDiMengObj"),
                                                new XAttribute("Name", "DiMengObj"));
            foreach (string cate in categoreis)
            {
                var category = new XElement("I",
                                   new XAttribute("Name", cate),
                                   new XAttribute("ToolTip", cate),
                                   new XAttribute("Photo", cate)
                                   );

                var products = repository.Products.Where(it => it.Category == cate).ToList();
                foreach (var p in products)
                {
                    if (products.IndexOf(p) == 0)
                    {
                        category.Attribute("Photo").Value = p.Id.ToString();
                    }

                    var productXml = new XElement("I",
                                        new XAttribute("Name", p.Name),
                                        new XAttribute("Photo", p.Id),
                                        new XAttribute("DWG", p.Id),
                                        new XAttribute("DMID", ""),
                                        new XAttribute("Units", "mm"),
                                        new XAttribute("Elevation", p.Elevation),
                                        new XAttribute("Z", p.Height),
                                        new XAttribute("Y", p.Depth),
                                        new XAttribute("X", p.Width),
                                        new XAttribute("LongText", p.Description),
                                        new XAttribute("ID", p.Id)
                                        );

                    category.Add(productXml);
                }

                dimengCategory.Add(category);
            }

            root.Add(dimengCategory);

            doc.Add(root);
            doc.Save(this.saveXMLPath);
        }
    }
}
