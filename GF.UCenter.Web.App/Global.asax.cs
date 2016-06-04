using System.ComponentModel.Composition.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using GF.UCenter.Common;
using GF.UCenter.Common.Settings;
using GF.UCenter.Web.Common;

namespace GF.UCenter.Web.App
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ExportProvider exportProvider = CompositionContainerFactory.Create();

            WebApplicationManager.InitializeApplication(GlobalConfiguration.Configuration, exportProvider);
            SettingsInitializer.Initialize<Settings>(
                exportProvider,
                SettingsDefaultValueProvider<Settings>.Default,
                AppConfigurationValueProvider.Default);
        }
    }
}
