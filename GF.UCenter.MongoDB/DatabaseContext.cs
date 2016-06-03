using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GF.UCenter.MongoDB.Adapters;
using GF.UCenter.MongoDB.Entity;
using MongoDB.Driver;

namespace GF.UCenter.MongoDB
{
    [Export]
    public class DatabaseContext
    {
        private readonly ExportProvider exportProvider;
        private readonly DatabaseContextSettings settings;
        private readonly IMongoClient client;
        private readonly IMongoDatabase database;

        [ImportingConstructor]
        private DatabaseContext(ExportProvider exportProvider, DatabaseContextSettings settings)
        {
            this.exportProvider = exportProvider;
            this.settings = settings;
            this.client = new MongoClient(settings.ConnectionString);
            this.database = client.GetDatabase(this.settings.DatabaseName);
        }

        public IMongoDatabase Database
        {
            get
            {
                return this.database;
            }
        }

        public DatabaseContextSettings Settings
        {
            get { return this.settings; }
        }

        public ICollectionAdapter<Account> Accounts
        {
            get
            {
                return this.GetAdapter<Account>();
            }
        }

        public ICollectionAdapter<App> Apps
        {
            get
            {
                return this.GetAdapter<App>();
            }
        }


        public ICollectionAdapter<AppAccountData> AppAccountDatas
        {
            get
            {
                return this.GetAdapter<AppAccountData>();
            }
        }

        public ICollectionAdapter<Order> Orders
        {
            get
            {
                return this.GetAdapter<Order>();
            }
        }

        public ICollectionAdapter<KeyPlaceholder> KeyPlaceholders
        {
            get
            {
                return this.GetAdapter<KeyPlaceholder>();
            }
        }

        public ICollectionAdapter<LoginRecord> LoginRecords
        {
            get
            {
                return this.GetAdapter<LoginRecord>();
            }
        }

        public ICollectionAdapter<TEntity> GetAdapter<TEntity>()
            where TEntity : EntityBase
        {
            return this.exportProvider.GetExportedValue<ICollectionAdapter<TEntity>>();
        }
    }
}
