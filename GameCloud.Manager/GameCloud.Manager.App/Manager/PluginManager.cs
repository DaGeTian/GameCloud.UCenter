using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Threading;
using GameCloud.Common.MEF;
using GameCloud.Common.Settings;
using GameCloud.Database;
using GameCloud.Database.Adapters;
using GameCloud.Manager.App.Models;
using GameCloud.Manager.Database;
using GameCloud.Manager.Database.Entities;
using Newtonsoft.Json;

namespace GameCloud.Manager.App.Manager
{
    public class PluginManager
    {
        private const string ManifestFileName = "manifest.json";
        private readonly ExportProvider exportProvider;
        private readonly ManagerDatabaseContext database;
        private readonly ConcurrentDictionary<string, PluginClient> clients = new ConcurrentDictionary<string, PluginClient>();
        private IReadOnlyList<Plugin> plugins;
        private readonly TimeSpan syncInterval = TimeSpan.FromSeconds(5);
        private readonly FileSystemWatcher watcher;
        private readonly string path;

        public PluginManager(string path)
        {
            this.path = path;
            this.exportProvider = CompositionContainerFactory.Create();
            SettingsInitializer.Initialize<DatabaseContextSettings>(
                this.exportProvider,
                SettingsDefaultValueProvider<DatabaseContextSettings>.Default,
                AppConfigurationValueProvider.Default);
            this.database = exportProvider.GetExportedValue<ManagerDatabaseContext>();
            this.plugins = this.GetPlugins(path);
            this.watcher = new FileSystemWatcher(path, "*" + ManifestFileName);
            this.watcher.IncludeSubdirectories = true;
            this.watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size;
            this.watcher.Changed += Watcher_Changed;
            this.watcher.Deleted += Watcher_Changed;
            this.watcher.Created += Watcher_Changed;
            this.watcher.EnableRaisingEvents = true;
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("Find file change: {0}", e.FullPath);
            this.SyncPlugins();
        }

        public IReadOnlyList<Plugin> Plugins
        {
            get
            {
                return this.plugins;
            }
        }

        public Plugin GetPlugin(string name)
        {
            return this.Plugins.FirstOrDefault(p => p.Name == name);
        }

        public PluginItem GetPluginItem(string pluginName, string categoryName, string itemName)
        {
            var plugin = this.Plugins.Single(p => p.Name == pluginName);
            var category = plugin.Categories.Single(c => c.Name == categoryName);
            var item = category.Items.Single(i => i.Name == itemName);

            return item;
        }

        public PluginClient GetClient(string pluginName)
        {
            return this.clients.GetOrAdd(pluginName, name =>
            {
                var plugin = this.Plugins.First(v => v.Name == name);
                return new PluginClient(plugin);
            });
        }

        private void SyncPlugins()
        {
            try
            {
                Console.WriteLine("Start to sync plugins.");
                var plugins = this.GetPlugins(this.path);
                this.plugins = plugins;
                Console.WriteLine("Finish sync pluings");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Sync plugins error. {0}", ex);
            }
        }

        private IReadOnlyList<Plugin> GetPlugins(string path)
        {
            var files = Directory.GetFiles(path, ManifestFileName, SearchOption.AllDirectories);
            var result = new List<Plugin>();
            foreach (var file in files)
            {
                var plugin = JsonConvert.DeserializeObject<Plugin>(File.ReadAllText(file));

                var demoPlugin = this.database.Plugins.GetSingleAsync(p => p.Name == "Demo", CancellationToken.None).Result;
                if (demoPlugin == null)
                {
                    this.database.Plugins.InsertAsync(new PluginEntity() { Id = "Demo", Name = "Demo", DisplayName = "Demo插件", Description = "Demo插件" }, CancellationToken.None).Wait();
                }
                var ucenterPlugin = this.database.Plugins.GetSingleAsync(p => p.Name == "UCenter", CancellationToken.None).Result;
                if (ucenterPlugin == null)
                {
                    this.database.Plugins.InsertAsync(new PluginEntity() { Id = "UCenter", Name = "UCenter", DisplayName = "UCenter插件", Description = "UCenter管理平台"}, CancellationToken.None).Wait();
                }

                // Database settings will override manifest settings;
                var pluginEntity = this.database.Plugins.GetSingleAsync(v => v.Name == plugin.Name, CancellationToken.None).Result;
                if (pluginEntity != null)
                {
                    plugin.Description = pluginEntity.Description;
                    plugin.DisplayName = pluginEntity.DisplayName;
                    plugin.Url = pluginEntity.Url;
                }
                result.Add(plugin);
            }

            return result;
        }
    }
}
