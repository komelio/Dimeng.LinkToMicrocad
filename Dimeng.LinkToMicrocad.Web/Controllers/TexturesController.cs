using Dimeng.LinkToMicrocad.Web.Domain.Abstract;
using Dimeng.LinkToMicrocad.Web.Domain.Entities;
using Dimeng.LinkToMicrocad.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dimeng.LinkToMicrocad.Web.Controllers
{
    public class TexturesController : Controller
    {
        ITextureRepository repository;
        int pageSize = 20;

        public TexturesController(ITextureRepository repo)
        {
            this.repository = repo;
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

        public ActionResult Edit(Texture texture)
        {
            return View();
        }
    }
}