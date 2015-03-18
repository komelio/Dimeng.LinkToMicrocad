using Dimeng.LinkToMicrocad.Web.Business;
using Dimeng.LinkToMicrocad.Web.Domain.Abstract;
using Dimeng.LinkToMicrocad.Web.Domain.Entities;
using Dimeng.LinkToMicrocad.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dimeng.LinkToMicrocad.Web.Controllers
{
    public class ProductsController : Controller
    {
        private Configuration configuration;
        private IProductRepository repository;
        private int pageSize = 20;

        public ProductsController(IProductRepository repo, Configuration config)
        {
            this.repository = repo;
            this.configuration = config;
        }

        public ViewResult List(string category, int page = 1)
        {
            ProductListViewModel model = new ProductListViewModel();
            model.Products = repository.Products.Where(p => category == null || category == p.Category)
                                                .OrderBy(it => it.Id)
                                                .Skip((page - 1) * pageSize)
                                                .Take(pageSize);
            model.PagingInfo = new PagingInfo()
            {
                CurrentPage = page,
                ItemsPerPage = pageSize,
                TotalItems = repository.Products
                                       .Where(it => category == null || it.Category == category)
                                       .Count()
            };
            model.CurrentCategory = category;

            return View(model);
        }

        public RedirectToRouteResult Import()
        {
            var pathOutput = Server.MapPath(configuration.PathToOutput);
            var pathLibrary = Server.MapPath(configuration.PathToLibrary);
            MVLibraryImporter importer = new MVLibraryImporter(repository, pathOutput, pathLibrary);
            importer.Import();

            return RedirectToAction("List");
        }

        public string Output()
        {
            string pathOutpu = Server.MapPath(configuration.PathToOutput);
            MVLibraryConverter converter = new MVLibraryConverter(repository, pathOutpu);
            converter.ConvertToXML();

            return "Done!";

            //return File(path, System.Net.Mime.MediaTypeNames.Application.Octet, "dms.xml");
        }

        public string TestAjax()
        {
            System.Threading.Thread.Sleep(2000);
            return "test ajax";
        }

        public RedirectToRouteResult Clear()
        {
            repository.Clear();

            return RedirectToAction("List");
        }

        [HttpGet]
        public ViewResult Edit(int productId)
        {
            Product product = repository.Products.FirstOrDefault(x => x.Id == productId);
            return View(product);
        }

        [HttpPost]
        public ActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.Id > 0)
                {
                    repository.ApplyModel(product);
                }
                else
                {
                    repository.Add(product);
                }
                return RedirectToAction("List");
            }
            else
            {
                return View(product);
            }
        }
    }
}