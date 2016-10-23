using System;
using System.ComponentModel.Composition.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using GameCloud.Database;
using GameCloud.UCenter.Common.Settings;
using GameCloud.UCenter.Web.Common.Dependency;
using GameCloud.UCenter.Web.Common.Filters;

namespace GameCloud.UCenter.Web.Common
{
    /// <summary>
    /// Provide a class to manage web application.
    /// </summary>
    public static class WebApplicationManager
    {
        /// <summary>
        /// Initialize the web application
        /// </summary>
        /// <param name="configuration">The http configuration</param>
        /// <param name="exportProvider">The export provider</param>
        /// <param name="callback">The callback function.</param>
        public static void InitializeApplication(
            HttpConfiguration configuration,
            ExportProvider exportProvider,
            Action<HttpConfiguration, ExportProvider> callback = null)
        {
            configuration.Filters.Add(new ActionExecutionFilterAttribute());
            RegisterMefDepencency(configuration, exportProvider);
            var controllerFactory = exportProvider.GetExportedValue<MefControllerFactory>();
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
            InitializeSettings(exportProvider);

            if (callback != null)
            {
                callback(configuration, exportProvider);
            }
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
            var dependency = new MefDependencyResolver(exportProvider, configuration.DependencyResolver);
            configuration.DependencyResolver = dependency;
        }
    }
}
