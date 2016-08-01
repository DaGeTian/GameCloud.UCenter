using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GF.Database;
using GF.Manager.Models;
using Newtonsoft.Json;

namespace GF.Manager.Handlers
{
    [Export]
    public class PluginManager
    {
        private List<Plugin> plugins = new List<Plugin>();
        private DateTime lastUpdateTime = DateTime.MinValue;
        private TimeSpan updateInterval = TimeSpan.FromMinutes(10);
        private readonly ConcurrentDictionary<Guid, PluginHandler> handlers = new ConcurrentDictionary<Guid, PluginHandler>();

        private readonly DatabaseContext context;

        [ImportingConstructor]
        public PluginManager(DatabaseContext context)
        {
            this.context = context;
        }

        public PluginHandler GetHandler(Plugin plugin, PluginItem item)
        {
            return this.handlers.GetOrAdd(
                item.Id,
                key => new PluginHandler(plugin, item));
        }

        public async Task<IReadOnlyList<Plugin>> GetPlugins(string userName, CancellationToken token)
        {
            await this.TryToSyncPlugins(token);
            // todo: filter plugins by user name.

            return this.plugins;
        }

        public async Task<Plugin> GetPlugin(string userName, Guid pluginId, CancellationToken token)
        {
            await this.TryToSyncPlugins(token);

            //todo: validate the user have right to manage the plugin.
            return this.plugins.Where(p => p.Id == pluginId).Single();
        }

        public PluginHandler GetHandler(Guid id)
        {
            // todo: generate plugin/pluginitem.
            var plugin = new Plugin();
            var pluginItem = new PluginItem();

            return this.GetHandler(plugin, pluginItem);
        }

        private async Task TryToSyncPlugins(CancellationToken token)
        {
            if (this.lastUpdateTime.Add(this.updateInterval) > DateTime.UtcNow)
            {
                var pluginList = await this.context.GetAdapter<PluginEntity>()
                    .GetListAsync(null, null, token);

                this.plugins = pluginList
                    .Select(p => JsonConvert.DeserializeObject<Plugin>(p.Content))
                    .ToList();
            }
        }
    }
}