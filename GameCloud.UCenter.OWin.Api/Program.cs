using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameCloud.Database.Adapters;
using GameCloud.UCenter.Common.MEF;
using GameCloud.UCenter.Common.Settings;
using GameCloud.UCenter.Database.Entities;
using GameCloud.UCenter.Web.Common;
using GameCloud.UCenter.Web.Common.Logger;
using Microsoft.Owin.Hosting;
using MongoDB.Driver;

namespace GameCloud.UCenter.OWin.Api
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = "http://localhost:5000/";

            // Start OWIN host 
            WebApp.Start<Startup>(url: baseAddress);

            //GlobalConfiguration.Configure(WebApiConfig.Register);

            // MEF initiliazation
            ExportProvider exportProvider = CompositionContainerFactory.Create();
            WebApplicationManager.InitializeApplication(Startup.HttpConfiguration, exportProvider);
            SettingsInitializer.Initialize<Settings>(
                exportProvider,
                SettingsDefaultValueProvider<Settings>.Default,
                AppConfigurationValueProvider.Default);

            try
            {
                var adapter = exportProvider.GetExportedValue<ICollectionAdapter<AccountEntity>>();
                adapter.CreateIndexIfNotExistAsync(
                    Builders<AccountEntity>.IndexKeys.Ascending("AccountName"),
                    new CreateIndexOptions() { Name = "AccountName_IDX" },
                    CancellationToken.None).Wait();
                adapter.CreateIndexIfNotExistAsync(
                    Builders<AccountEntity>.IndexKeys.Ascending("Email"),
                    new CreateIndexOptions() { Name = "Email_IDX" },
                    CancellationToken.None).Wait();
                adapter.CreateIndexIfNotExistAsync(
                    Builders<AccountEntity>.IndexKeys.Ascending("Phone"),
                    new CreateIndexOptions() { Name = "Phone_IDX" },
                    CancellationToken.None).Wait();
            }
            catch (Exception ex)
            {
                CustomTrace.TraceError(ex);
            }

            // Create HttpCient and make a request to api/values 
            HttpClient client = new HttpClient();

            //var response = client.GetAsync(baseAddress + "api/accounts/ip").Result;
            var response = client.GetAsync(baseAddress + "api/apps/texaspoker/configurations").Result;

            Console.WriteLine(response);
            Console.WriteLine(response.Content.ReadAsStringAsync().Result);

            Console.ReadLine();
        }
    }
}
