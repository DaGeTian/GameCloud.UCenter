namespace GF.UCenter.MongoDB
{
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using Adapters;
    using Entity;
    using global::MongoDB.Driver;

    [Export]
    public class DatabaseContext
    {
        private readonly ExportProvider exportProvider;

        [ImportingConstructor]
        private DatabaseContext(ExportProvider exportProvider, DatabaseContextSettings settings)
        {
            this.exportProvider = exportProvider;
            this.Settings = settings;
            var client = new MongoClient(settings.ConnectionString);
            this.Database = client.GetDatabase(this.Settings.DatabaseName);
        }

        public IMongoDatabase Database { get; }

        public DatabaseContextSettings Settings { get; }

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
