using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CodeVision.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute(
                name: "FourParams",
                url: "{controller}/{action}/{searchExpressionEncoded}/{language}/{sort}/{page}");

            routes.MapRoute(
                name: "ThreeParams",
                url: "{controller}/{action}/{searchExpressionEncoded}/{language}/{page}");

            routes.MapRoute(
                name: "TwoParams",
                url: "{controller}/{action}/{searchExpressionEncoded}/{page}");
            
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
