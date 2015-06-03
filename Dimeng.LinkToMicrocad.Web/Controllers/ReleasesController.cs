using Dimeng.LinkToMicrocad.Web.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dimeng.LinkToMicrocad.Web.Controllers
{
    public class ReleasesController : Controller
    {
        // GET: Releases
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LatestReleaseVersion()
        {
            Release release = new Release();
            return Json(release, JsonRequestBehavior.AllowGet);
        }
    }
}