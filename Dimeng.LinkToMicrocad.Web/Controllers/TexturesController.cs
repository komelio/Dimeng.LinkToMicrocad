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
    public class TexturesController : Controller
    {
        ITextureRepository repository;
        Configuration configuration;
        int pageSize = 20;

        public TexturesController(ITextureRepository repo, Configuration configuration)
        {
            this.repository = repo;
            this.configuration = configuration;
        }
        public ActionResult List(int page = 1)
        {
            TextureListViewModel model = new TextureListViewModel();
            model.Textures = repository.Textures
                                                .OrderBy(it => it.TextureId)
                                                .Skip((page - 1) * pageSize)
                                                .Take(pageSize);
            model.PagingInfo = new PagingInfo()
            {
                CurrentPage = page,
                ItemsPerPage = pageSize,
                TotalItems = repository.Textures
                                       .Count()
            };
            //model.CurrentCategory = category;

            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int textureId = 0)
        {
            if (textureId == 0)
            {
                ViewBag.Title = "创建新材质";
                return View(new Texture());
            }
            ViewBag.Title = "修改材质";
            var texture = repository.Textures.FirstOrDefault(t => t.TextureId == textureId);
            return View(texture);
        }

        [HttpPost]
        public ActionResult Edit(Texture texture, HttpPostedFileBase image = null)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    texture.ImageName = image.FileName.Substring(image.FileName.LastIndexOf("\\") + 1);
                    string savePath = Server.MapPath(configuration.PathToOutput);
                    image.SaveAs(Path.Combine(savePath, "Material", "high", texture.ImageName));
                    //TempData["message"] = "done!";
                }

                if (texture.TextureId == 0)
                {
                    repository.Add(texture);
                }
                else
                {
                    repository.ApplyModel(texture);
                }

                return RedirectToAction("List");
            }
            else
            {
                return View(texture);
            }
        }

        public ViewResult Create()
        {
            return View("Edit", new Texture());
        }

        public ActionResult Delete(int textureId)
        {
            if (textureId == 0)
            {
                HttpNotFound();
            }
            else
            {
                repository.Delete(textureId);
            }
            return RedirectToAction("List");
        }
    }
}