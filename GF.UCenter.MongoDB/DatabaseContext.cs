using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
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

        public ICollectionAdapter<AccountEntity> Accounts
        {
            get
            {
                return this.GetAdapter<AccountEntity>();
            }
        }

        public ICollectionAdapter<AppEntity> Apps
        {
            get
            {
                return this.GetAdapter<AppEntity>();
            }
        }


        public ICollectionAdapter<AppAccountDataEntity> AppAccountDatas
        {
            get
            {
                return this.GetAdapter<AppAccountDataEntity>();
            }
        }

        public ICollectionAdapter<OrderEntity> Orders
        {
            get
            {
                return this.GetAdapter<OrderEntity>();
            }
        }

        public ICollectionAdapter<KeyPlaceholderEntity> KeyPlaceholders
        {
            get
            {
                return this.GetAdapter<KeyPlaceholderEntity>();
            }
        }

        public ICollectionAdapter<LoginRecordEntity> LoginRecords
        {
            get
            {
                return this.GetAdapter<LoginRecordEntity>();
            }
        }

        public ICollectionAdapter<TEntity> GetAdapter<TEntity>()
            where TEntity : EntityBase
        {
            return this.exportProvider.GetExportedValue<ICollectionAdapter<TEntity>>();
        }
    }
}
