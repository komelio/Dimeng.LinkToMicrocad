﻿using Dimeng.LinkToMicrocad.Web.Business;
using Dimeng.LinkToMicrocad.Web.Domain.Abstract;
using Dimeng.LinkToMicrocad.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dimeng.LinkToMicrocad.Web.Controllers
{
    public class MaterialsController : Controller
    {
        IMaterialRepository repository;
        private int pageSize = 20;

        public MaterialsController(IMaterialRepository repo)
        {
            this.repository = repo;
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

        public ActionResult Import()
        {
            string file = @"D:\MV\Template\切割板件文件.ctpx";
            MVMaterialImporter importer = new MVMaterialImporter(repository);
            importer.Import(file);

            return RedirectToAction("List");
        }
    }
}