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
                routeTemplate: "api/GetEtaByShip/{id}",
                defaults: new { controller = "ShipName", action = "GetEtaByShip", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
