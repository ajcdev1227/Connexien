using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Connexien.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "P",
                url: "P/{id}/{action}",
                defaults: new { controller = "P", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "A",
                url: "A/{id}",
                defaults: new { controller = "A", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Profile",
                url: "Profile/{id}",
                defaults: new { controller = "Profile", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );


        }
    }
}