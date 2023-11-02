using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EServicesCms
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            if (Common.WebConfig.GetBoolValue("EnableSSO_Authentication"))
            {
                routes.MapRoute(
                        name: "Default",
                        url: "{controller}/{action}/{id}",
                        defaults: new { controller = "Auth", action = "Login", id = UrlParameter.Optional }
                    );
            }
            else
            {
                routes.MapRoute(
                        name: "Default",
                        url: "{controller}/{action}/{id}",
                        defaults: new { controller = "Account", action = "Index", id = UrlParameter.Optional }
                    );
            }
            
        }
    }
}
