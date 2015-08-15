using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using Funq;
using ServiceStack;
using ServiceStack.Razor;
using Yaqaap.Framework;
using Yaqaap.ServiceInterface;

namespace Yaqaap
{
    public class AppHost : AppHostBase
    {
        /// <summary>
        /// Default constructor.
        /// Base constructor requires a name and assembly to locate web service classes. 
        /// </summary>
        public AppHost()
            : base("Yaqaap", typeof(MyServices).Assembly)
        {

        }

        /// <summary>
        /// Application specific configuration
        /// This method should initialize any IoC resources utilized by your web service classes.
        /// </summary>
        /// <param name="container"></param>
        public override void Configure(Container container)
        {
            //Config examples
            //this.Plugins.Add(new PostmanFeature());
            //this.Plugins.Add(new CorsFeature());

            this.Plugins.Add(new RazorFormat());

            // Hack pour que le F5 fonctionne avec angular-route
            this.CustomErrorHttpHandlers[HttpStatusCode.NotFound] = new RazorHandler("/");
            //this.CustomErrorHttpHandlers[HttpStatusCode.Unauthorized] = new RazorHandler("/login");

            StorageConfig.StorageConnexionString = ConfigurationManager.AppSettings["storage"];

        }

        public override RouteAttribute[] GetRouteAttributes(Type requestType)
        {
            var routes = base.GetRouteAttributes(requestType);
            routes.Each(x => x.Path = "/api" + x.Path);
            return routes;
        }

    }
}