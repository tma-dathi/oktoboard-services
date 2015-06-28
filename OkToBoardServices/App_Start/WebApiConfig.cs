using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace OkToBoardServices
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "CustomApi",
                routeTemplate: "api/shipname",
                defaults: new { controller = "Vessel" }
            );

            config.Routes.MapHttpRoute(
                name: "CustomApi1",
                routeTemplate: "api/GetEtaByShip/{id}",
                defaults: new { controller = "Vessel", action = "GetEtaByShip", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "ImageApi",
                routeTemplate: "api/postimage/{id}",
                defaults: new { controller = "Vessel", action = "PostImage", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
