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

        [ImportingConstructor]
        private DatabaseContext(ExportProvider exportProvider, DatabaseContextSettings settings)
        {
            this.exportProvider = exportProvider;
            this.settings = settings;
            var client = new MongoClient(settings.ConnectionString);
            this.Database = client.GetDatabase(this.settings.DatabaseName);
        }

        public IMongoDatabase Database { get; }

        public DatabaseContextSettings Settings => this.settings;

        public ICollectionAdapter<AccountEntity> Accounts => this.GetAdapter<AccountEntity>();

        public ICollectionAdapter<AppEntity> Apps => this.GetAdapter<AppEntity>();

        public ICollectionAdapter<AppAccountDataEntity> AppAccountDatas => this.GetAdapter<AppAccountDataEntity>();

        public ICollectionAdapter<OrderEntity> Orders => this.GetAdapter<OrderEntity>();

        public ICollectionAdapter<KeyPlaceholderEntity> KeyPlaceholders => this.GetAdapter<KeyPlaceholderEntity>();

        public ICollectionAdapter<LoginRecordEntity> LoginRecords => this.GetAdapter<LoginRecordEntity>();

        public ICollectionAdapter<TEntity> GetAdapter<TEntity>()
            where TEntity : EntityBase
        {
            return this.exportProvider.GetExportedValue<ICollectionAdapter<TEntity>>();
        }
    }
}
