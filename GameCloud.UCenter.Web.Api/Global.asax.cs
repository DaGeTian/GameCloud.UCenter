using System;
using System.ComponentModel.Composition.Hosting;
using System.Threading;
using System.Web;
using System.Web.Http;
using GameCloud.Database.Adapters;
using GameCloud.UCenter.Common.MEF;
using GameCloud.UCenter.Common.Settings;
using GameCloud.UCenter.Database.Entities;
using GameCloud.UCenter.Web.Common;
using GameCloud.UCenter.Web.Common.Logger;
using MongoDB.Driver;

namespace GameCloud.UCenter.Web.Api
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
            CustomTrace.Initialize(exportProvider, "Trace.NLog");

            // Mongo db index creation
            try
            {
                var adapter = exportProvider.GetExportedValue<ICollectionAdapter<AccountEntity>>();
                adapter.CreateIndexIfNotExistAsync(
                    Builders<AccountEntity>.IndexKeys.Ascending("AccountName"),
                    new CreateIndexOptions() { Name = "AccountName_UI", Unique = true },
                    CancellationToken.None).Wait();
                adapter.CreateIndexIfNotExistAsync(
                    Builders<AccountEntity>.IndexKeys.Ascending("Email"),
                    new CreateIndexOptions() { Name = "Email_UI", Unique = true },
                    CancellationToken.None).Wait();
                adapter.CreateIndexIfNotExistAsync(
                    Builders<AccountEntity>.IndexKeys.Ascending("Phone"),
                    new CreateIndexOptions() { Name = "Phone_UI", Unique = true },
                    CancellationToken.None).Wait();
            }
            catch (Exception ex)
            {
            }
        }
    }
}