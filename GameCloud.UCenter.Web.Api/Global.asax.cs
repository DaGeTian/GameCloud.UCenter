using System.ComponentModel.Composition.Hosting;
using System.Web;
using System.Web.Http;
using GameCloud.UCenter;
using GameCloud.UCenter;
using GameCloud.UCenter;

namespace GameCloud.UCenter
{
    /// <summary>
    /// MVC Application
    /// </summary>
    public class WebApiApplication : HttpApplication
    {
        /// <summary>
        /// The application start event
        /// </summary>
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            // MEF initiliazation
            ExportProvider exportProvider = CompositionContainerFactory.Create();
            WebApplicationManager.InitializeApplication(GlobalConfiguration.Configuration, exportProvider);
            SettingsInitializer.Initialize<Settings>(
                exportProvider,
                SettingsDefaultValueProvider<Settings>.Default,
                AppConfigurationValueProvider.Default);
        }
    }
}