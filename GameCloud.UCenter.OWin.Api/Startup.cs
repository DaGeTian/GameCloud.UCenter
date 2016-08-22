using System.Web.Http;
using Owin;

namespace GameCloud.UCenter.OWin.Api
{
    class Startup
    {
        public static HttpConfiguration HttpConfiguration { get; private set; }

        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration = new HttpConfiguration();
            HttpConfiguration.MapHttpAttributeRoutes();
            HttpConfiguration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            appBuilder.UseWebApi(HttpConfiguration);
        }
    }
}
