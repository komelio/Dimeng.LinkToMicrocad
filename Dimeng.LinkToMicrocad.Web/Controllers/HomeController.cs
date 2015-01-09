using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dimeng.LinkToMicrocad.Web.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ViewResult Index()
        {
            ViewBag.Greeting = "good moring!";

            return View();
        }

        public ViewResult AddMVProductForm()
        {
            return View();
        }
    }
}