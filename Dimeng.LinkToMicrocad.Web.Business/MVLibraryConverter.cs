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
        public MVLibraryConverter(IProductRepository repo)
        {
            this.repository = repo;
        }

        public string ConvertToXML()
        {
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

                foreach (var p in repository.Products.Where(it => it.Category == cate))
                {
                    var productXml = new XElement("I",
                                        new XAttribute("Name", p.Name),
                                        new XAttribute("Photo", p.Name),
                                        new XAttribute("DWG", p.Id),
                                        new XAttribute("DMID", ""),
                                        new XAttribute("Units", "mm"),
                                        new XAttribute("Elevation", ""),
                                        new XAttribute("Z", "400"),
                                        new XAttribute("Y", "500"),
                                        new XAttribute("X", "400"),
                                        new XAttribute("LongText", p.Description),
                                        new XAttribute("ID", p.Id)
                                        );

                    category.Add(productXml);
                }

                dimengCategory.Add(category);
            }

            root.Add(dimengCategory);

            doc.Add(root);

            string path = @"c:\temp.xml";

            doc.Save(path);
            return path;
        }
    }
}
