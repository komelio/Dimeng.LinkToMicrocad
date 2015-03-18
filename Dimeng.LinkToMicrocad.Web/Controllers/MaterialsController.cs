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
using System.Xml.Linq;

namespace Dimeng.LinkToMicrocad.Web.Controllers
{
    public class MaterialsController : Controller
    {
        IMaterialRepository repository;
        ITextureRepository textureRepository;
        private int pageSize = 20;
        private Configuration configuration;

        public MaterialsController(IMaterialRepository repo, ITextureRepository textureRepository, Configuration config)
        {
            this.textureRepository = textureRepository;
            this.repository = repo;
            this.configuration = config;
        }

        // GET: Material
        public ActionResult Index()
        {
            return View("List");
        }

        public ActionResult List(int page = 1)
        {
            MaterialListViewModel model = new MaterialListViewModel();
            model.Materials = repository.Materials
                                                .OrderBy(it => it.MaterialId)
                                                .Skip((page - 1) * pageSize)
                                                .Take(pageSize);
            model.PagingInfo = new PagingInfo()
            {
                CurrentPage = page,
                ItemsPerPage = pageSize,
                TotalItems = repository.Materials
                                       .Count()
            };
            //model.CurrentCategory = category;

            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            var material = repository.Materials.FirstOrDefault(m => m.MaterialId == id);

            var textureQry = textureRepository.Textures
                                              .Select(it =>
                                                  {
                                                      bool isSelected = false;
                                                      if (material != null && material.TextureId == it.TextureId)
                                                      {
                                                          isSelected = true;
                                                      }
                                                      return new SelectListItem()
                                                          {
                                                              Value = it.TextureId.ToString(),
                                                              Text = it.Name,
                                                              Selected = isSelected
                                                          };
                                                  })
                                              .ToList();

            ViewData["selection"] = new SelectList(textureQry, "Value", "Text");

            return View(material);
        }

        [HttpPost]
        public ActionResult Edit(Material material, int? textureId)
        {
            if (ModelState.IsValid)
            {
                material.TextureId = textureId;
                repository.ApplyModel(material);
                return RedirectToAction("List");
            }

            return View(material);
        }

        public ActionResult Import()
        {
            string file = @"D:\MV\Template\切割板件文件.ctpx";

            MVMaterialImporter importer = new MVMaterialImporter(repository);
            importer.Import(file);

            return RedirectToAction("List");
        }

        public ActionResult Output()
        {
            string path = Server.MapPath(configuration.PathToOutput);
            path = Path.Combine(path, "Library", "materials.xml");

            XDocument doc = new XDocument();
            XElement xml = new XElement("Materials");

            foreach (var m in repository.Materials)
            {
                if (m.TextureId == null || m.TextureId == 0)
                {
                    continue;
                }

                XElement xmlMaterial = new XElement("Material",
                                            new XAttribute("Name", m.Name),
                                            new XAttribute("Texture", m.Texture.ImageName));
                xml.Add(xmlMaterial);
            }
            doc.Add(xml);
            doc.Save(path);

            TempData["msg"] = "<script>alert('Change succesfully');</script>";
            return RedirectToAction("List");
        }
    }
}