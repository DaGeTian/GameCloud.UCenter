using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Threading.Tasks;
using GameCloud.Database;
using GameCloud.UCenter.Common.MEF;
using GameCloud.UCenter.Common.Settings;
using GameCloud.UCenter.Web.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using GameCloud.UCenter.Database;
using GameCloud.UCenter.Web.Common.Storage;
using GameCloud.UCenter.Web.Common.Logger;

namespace GameCloud.UCenter.Api
{
    public class Startup
    {
        private readonly ExportProvider exportProvider;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            // MEF initiliazation
            this.exportProvider = CompositionContainerFactory.Create();
            //WebApplicationManager.InitializeApplication(GlobalConfiguration.Configuration, exportProvider);
            SettingsInitializer.Initialize<Settings>(
                this.exportProvider,
                SettingsDefaultValueProvider<Settings>.Default,
                AppConfigurationValueProvider.Default);
            SettingsInitializer.Initialize<DatabaseContextSettings>(
                this.exportProvider,
                SettingsDefaultValueProvider<DatabaseContextSettings>.Default,
                AppConfigurationValueProvider.Default);
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            services.AddSingleton<Settings>(this.exportProvider.GetExportedValue<Settings>());
            services.AddSingleton<StorageAccountContext>(this.exportProvider.GetExportedValue<StorageAccountContext>());
            services.AddSingleton<EventTrace>(this.exportProvider.GetExportedValue<EventTrace>());
            services.AddSingleton<UCenterDatabaseContext>(this.exportProvider.GetExportedValue<UCenterDatabaseContext>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
        }
    }
}
