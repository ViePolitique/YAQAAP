using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using Funq;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Caching;
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

            Plugins.Add(new AuthFeature(() => new AuthUserSession(),
                  new IAuthProvider[] {
                    new BasicAuthProvider(), //Sign-in with HTTP Basic Auth
                    new AzureAuthProvider(), //HTML Form post of UserName/Password credentials
                  }));

            Plugins.Add(new RegistrationFeature());

            container.Register<ICacheClient>(new MemoryCacheClient());
            var userRep = new InMemoryAuthRepository();
            container.Register<IUserAuthRepository>(userRep);

            // demo account ////////////
            userRep.CreateUserAuth(new UserAuth()
                                   {
                                       Email = "demo@yaqaap.com",
                                       UserName = "demo"
                                   }, "0000");
            ////////////////////////////

            this.Plugins.Add(new RazorFormat());

            // Hack pour que le F5 fonctionne avec angular-route
            this.CustomErrorHttpHandlers[HttpStatusCode.NotFound] = new RazorHandler("/");
            //this.CustomErrorHttpHandlers[HttpStatusCode.Unauthorized] = new RazorHandler("/login");

#if DEBUG
            StorageConfig.StorageConnexionString = null;
#else
            StorageConfig.StorageConnexionString = ConfigurationManager.AppSettings["storage"];
#endif

        }

        public override RouteAttribute[] GetRouteAttributes(Type requestType)
        {
            var routes = base.GetRouteAttributes(requestType);
            routes.Each(x => x.Path = "/api" + x.Path);
            return routes;
        }

    }
}