using System.Web.Http;

namespace GameCloud.UCenter.Web.Api
{
    /// <summary>
    /// The web API configuration
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Register http configuration
        /// </summary>
        /// <param name="config">The http configuration.</param>
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi", 
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional });
        }
    }
}