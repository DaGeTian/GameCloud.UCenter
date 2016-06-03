namespace GF.UCenter.Web.Common
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Mvc;
    using GF.UCenter.Common.Settings;
    using MongoDB;
    public static class WebApplicationManager
    {
        /// <summary>
        /// Initialize the web application
        /// </summary>
        /// <param name="configuration">The http configuration</param>
        /// <param name="exportProvider">The export provider</param>
        public static void InitializeApplication(HttpConfiguration configuration, ExportProvider exportProvider)
        {
            configuration.Filters.Add(new ActionExecutionFilterAttribute());
            RegisterMefDepencency(configuration, exportProvider);
            ControllerBuilder.Current.SetControllerFactory(new MefControllerFactory(exportProvider));
            InitializeSettings(exportProvider);
        }

        private static void InitializeSettings(ExportProvider exportProvider)
        {
            SettingsInitializer.Initialize<Settings>(
                exportProvider,
                SettingsDefaultValueProvider<Settings>.Default,
                AppConfigurationValueProvider.Default);

            SettingsInitializer.Initialize<DatabaseContextSettings>(
                exportProvider,
                SettingsDefaultValueProvider<DatabaseContextSettings>.Default,
                AppConfigurationValueProvider.Default);
        }

        private static void RegisterMefDepencency(HttpConfiguration configuration, ExportProvider exportProvider)
        {
            var dependency = new MefDependencyResolver(exportProvider);
            configuration.DependencyResolver = dependency;
        }
    }

}
