using Dimeng.LinkToMicrocad.Web.Domain.Concrete;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Dimeng.LinkToMicrocad.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);


            Database.SetInitializer<EFDbContext>(new DropCreateDatabaseAlways<EFDbContext>());

            // Forces initialization of database on model changes.
            using (var context = new EFDbContext())
            {
                context.Database.Initialize(force: true);
            }    
        }
    }
}
