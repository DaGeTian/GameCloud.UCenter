using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using GameCloud.Database;
using GameCloud.Database.Adapters;
using GameCloud.Manager.Database.Entities;

namespace GameCloud.Manager.Database
{
    [Export]
    public class ManagerDatabaseContext : DatabaseContext
    {
        [ImportingConstructor]
        public ManagerDatabaseContext(ExportProvider exportProvider, DatabaseContextSettings settings)
            : base(exportProvider, settings)
        {
        }

        /// <summary>
        /// Gets the plugin collection adapter.
        /// </summary>
        public ICollectionAdapter<PluginEntity> Plugins
        {
            get
            {
                return this.GetAdapter<PluginEntity>();
            }
        }
    }
}
