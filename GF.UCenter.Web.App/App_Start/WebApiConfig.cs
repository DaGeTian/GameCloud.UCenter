namespace GF.UCenter.Web.App
{
    using System.Web.Http;

    internal static class WebApiConfig
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

            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });
        }
    }
}