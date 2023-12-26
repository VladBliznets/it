using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace RESTAPISERVER
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/javascript"));
        }
    }
}
