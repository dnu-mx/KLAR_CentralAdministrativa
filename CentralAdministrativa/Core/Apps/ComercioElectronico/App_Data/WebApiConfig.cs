using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using Newtonsoft.Json;

namespace ComercioElectronico.App_data
    
{
    public static class WebApiConfig 
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);


            config.Routes.MapHttpRoute(name: "API Default", routeTemplate: "api/{controller}/{action}/{id}", defaults: new { id = RouteParameter.Optional });

            //config.Routes.MapHttpRoute("DefaultApiWithId", "Api/{controller}/{id}", new { id = RouteParameter.Optional }, new { id = @"\w+" });
            config.Routes.MapHttpRoute("DefaultApiWithId", "Api/{controller}/{id}", new { id = RouteParameter.Optional }, new { id = @"\d+" });
            config.Routes.MapHttpRoute("DefaultApiWithAction", "Api/{controller}/{action}/");


            config.Routes.MapHttpRoute("DefaultApiGet", "Api/{controller}", new { action = "Get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("DefaultApiPost", "Api/{controller}", new { action = "Post" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            config.Routes.MapHttpRoute("DefaultApiPut", "Api/{controller}", new { action = "Put" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            //GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Re‌ferenceLoopHandling = ReferenceLoopHandling.Ignore;

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings =
new JsonSerializerSettings
{
    DateFormatHandling = DateFormatHandling.IsoDateFormat,    
    DateTimeZoneHandling  =DateTimeZoneHandling.Local
    //DateTimeZoneHandling.Local,
    //DateFormatString = DateFormatHandling,
};

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;

        }



    }






}
