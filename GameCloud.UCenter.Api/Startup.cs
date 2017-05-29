using System.ComponentModel.Composition.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Serilog;
using GameCloud.Common.MEF;
using GameCloud.Common.Settings;
using GameCloud.Database;
using GameCloud.UCenter.Common.Settings;
using GameCloud.UCenter.Database;
using GameCloud.UCenter.Database.Entities;
using GameCloud.UCenter.Web.Common.Logger;
using GameCloud.UCenter.Web.Common.Storage;

namespace GameCloud.UCenter.Api
{
    public class Startup
    {
        //---------------------------------------------------------------------
        private readonly ExportProvider exportProvider;
        public IConfigurationRoot Configuration { get; }

        //---------------------------------------------------------------------
        public Startup(IHostingEnvironment env)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.LiterateConsole()
                .CreateLogger();

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            // MEF initiliazation
            this.exportProvider = CompositionContainerFactory.Create();
            SettingsInitializer.Initialize<Settings>(
                this.exportProvider,
                SettingsDefaultValueProvider<Settings>.Default,
                AppConfigurationValueProvider.Default);
            SettingsInitializer.Initialize<DatabaseContextSettings>(
                this.exportProvider,
                SettingsDefaultValueProvider<DatabaseContextSettings>.Default,
                AppConfigurationValueProvider.Default);
            SettingsInitializer.Initialize<UCenterEventDatabaseContextSettings>(
                this.exportProvider,
                SettingsDefaultValueProvider<UCenterEventDatabaseContextSettings>.Default,
                AppConfigurationValueProvider.Default);

            CreateDatabaseIndexesAsync().Wait();
        }

        //---------------------------------------------------------------------
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            var settings = this.exportProvider.GetExportedValue<Settings>();
            services.AddSingleton<Settings>(settings);
            services.AddSingleton<IStorageContext>(this.exportProvider.GetExportedValue<IStorageContext>(settings.StorageType));
            services.AddSingleton<EventTrace>(this.exportProvider.GetExportedValue<EventTrace>());
            services.AddSingleton<UCenterDatabaseContext>(this.exportProvider.GetExportedValue<UCenterDatabaseContext>());
            services.AddSingleton<UCenterEventDatabaseContext>(this.exportProvider.GetExportedValue<UCenterEventDatabaseContext>());
        }

        //---------------------------------------------------------------------
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IApplicationLifetime appLifetime)
        {
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();
            loggerFactory.AddSerilog();

            // Ensure any buffered events are sent at shutdown
            appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);

            app.UseMvc();
            //app.UseStaticFiles();
        }

        //---------------------------------------------------------------------
        private async Task CreateDatabaseIndexesAsync()
        {
            var db = exportProvider.GetExportedValue<UCenterDatabaseContext>();

            await db.Accounts.CreateIndexIfNotExistAsync(
                Builders<AccountEntity>.IndexKeys.Ascending("AccountName"),
                new CreateIndexOptions() { Name = "AccountName_IDX" },
                CancellationToken.None);
            await db.Accounts.CreateIndexIfNotExistAsync(
                Builders<AccountEntity>.IndexKeys.Ascending("Email"),
                new CreateIndexOptions() { Name = "Email_IDX" },
                CancellationToken.None);
            await db.Accounts.CreateIndexIfNotExistAsync(
                Builders<AccountEntity>.IndexKeys.Ascending("Phone"),
                new CreateIndexOptions() { Name = "Phone_IDX" },
                CancellationToken.None);

            await db.AccountWechat.CreateIndexIfNotExistAsync(
                Builders<AccountWechatEntity>.IndexKeys.Ascending("AccountId"),
                new CreateIndexOptions() { Name = "AccountId_IDX" },
                CancellationToken.None);
            await db.AccountWechat.CreateIndexIfNotExistAsync(
                Builders<AccountWechatEntity>.IndexKeys.Ascending("Unionid"),
                new CreateIndexOptions() { Name = "Unionid_IDX" },
                CancellationToken.None);
            await db.AccountWechat.CreateIndexIfNotExistAsync(
                Builders<AccountWechatEntity>.IndexKeys.Ascending("OpenId"),
                new CreateIndexOptions() { Name = "OpenId_IDX" },
                CancellationToken.None);
            await db.AccountWechat.CreateIndexIfNotExistAsync(
                Builders<AccountWechatEntity>.IndexKeys.Ascending("AppId"),
                new CreateIndexOptions() { Name = "AppId_IDX" },
                CancellationToken.None);
        }
    }
}
