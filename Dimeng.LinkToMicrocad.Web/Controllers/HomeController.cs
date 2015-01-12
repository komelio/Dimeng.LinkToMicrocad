using Dimeng.LinkToMicrocad.Web.Models;
using Ninject;
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
            return View();
        }

        [HttpGet]
        public ViewResult AddMVProductForm()
        {
            return View();
        }

        [HttpPost]
        public ViewResult AddMVProductForm(MVProduct product)
        {
            if(ModelState.IsValid)
            {
                return View("Thanks", product);
            }
            else
            {
                return View();
            }
            
        }
    }
}