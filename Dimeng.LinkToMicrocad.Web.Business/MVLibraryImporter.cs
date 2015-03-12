using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Dimeng.LinkToMicrocad.Web.Domain.Entities;
using Dimeng.LinkToMicrocad.Web.Domain.Abstract;

namespace Dimeng.LinkToMicrocad.Web.Business
{
    public class MVLibraryImporter
    {
        private string sourcePath;
        private string savePath;
        private IProductRepository repository;

        public MVLibraryImporter(IProductRepository repo, string savePath, string sourcePath)
        {
            this.repository = repo;
            this.sourcePath = sourcePath;
            this.savePath = savePath;
        }

        public void Import()
        {
            string[] directories = Directory.GetDirectories(sourcePath);
            foreach (string dir in directories)
            {
                DirectoryInfo di = new DirectoryInfo(dir);
                foreach (var fi in di.GetFiles("*.cutx"))
                {
                    Product product = new Product();
                    product.Category = di.Name;
                    product.Description = string.Empty;
                    product.Name = fi.Name.Substring(0, fi.Name.LastIndexOf(fi.Extension));
                    product.Width = 400;
                    product.Height = 600;
                    product.Depth = 400;
                    product.Elevation = 0;

                    int id = repository.Add(product);

                    File.Copy(fi.FullName, Path.Combine(savePath, string.Format("{0}.cutx", id)), false);

                    var pictures = di.GetFiles(product.Name + ".jpg");
                    if (pictures.Length > 0)
                    {
                        File.Copy(pictures[0].FullName, Path.Combine(savePath, string.Format("{0}.jpg", id)), false);
                    }
                }
            }
        }
    }
}
