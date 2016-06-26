namespace GF.UCenter.MongoDB
{
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using Adapters;
    using Entity;
    using global::MongoDB.Driver;

    /// <summary>
    /// Provide a class for database context.
    /// </summary>
    [Export]
    public class DatabaseContext
    {
        private readonly ExportProvider exportProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContext" /> class.
        /// </summary>
        /// <param name="exportProvider">Indicating the export provider.</param>
        /// <param name="settings">Indicating the database context settings.</param>
        [ImportingConstructor]
        private DatabaseContext(ExportProvider exportProvider, DatabaseContextSettings settings)
        {
            this.exportProvider = exportProvider;
            this.Settings = settings;
            var client = new MongoClient("mongodb://42.159.239.155:27017");//settings.ConnectionString);
            this.Database = client.GetDatabase("ucenter_test"); //this.Settings.DatabaseName);
        }

        /// <summary>
        /// Gets the database.
        /// </summary>
        public IMongoDatabase Database { get; }

        /// <summary>
        /// Gets the database context settings.
        /// </summary>
        public DatabaseContextSettings Settings { get; }

        /// <summary>
        /// Gets the account adapter.
        /// </summary>
        public ICollectionAdapter<AccountEntity> Accounts
        {
            get
            {
                return this.GetAdapter<AccountEntity>();
            }
        }

        /// <summary>
        /// Gets the application adapter.
        /// </summary>
        public ICollectionAdapter<AppConfigurationEntity> AppConfigurations
        {
            get
            {
                return this.GetAdapter<AppConfigurationEntity>();
            }
        }

        /// <summary>
        /// Gets the application adapter.
        /// </summary>
        public ICollectionAdapter<AppEntity> Apps
        {
            get
            {
                return this.GetAdapter<AppEntity>();
            }
        }

        /// <summary>
        /// Gets the account application data adapter.
        /// </summary>
        public ICollectionAdapter<AppAccountDataEntity> AppAccountDatas
        {
            get
            {
                return this.GetAdapter<AppAccountDataEntity>();
            }
        }

        /// <summary>
        /// Gets the order adapter.
        /// </summary>
        public ICollectionAdapter<OrderEntity> Orders
        {
            get
            {
                return this.GetAdapter<OrderEntity>();
            }
        }

        /// <summary>
        /// Gets the key placeholder adapter.
        /// </summary>
        public ICollectionAdapter<KeyPlaceholderEntity> KeyPlaceholders
        {
            get
            {
                return this.GetAdapter<KeyPlaceholderEntity>();
            }
        }

        /// <summary>
        /// Gets the login record adapter.
        /// </summary>
        public ICollectionAdapter<LoginRecordEntity> LoginRecords
        {
            get
            {
                return this.GetAdapter<LoginRecordEntity>();
            }
        }

        private ICollectionAdapter<TEntity> GetAdapter<TEntity>()
            where TEntity : EntityBase
        {
            return this.exportProvider.GetExportedValue<ICollectionAdapter<TEntity>>();
        }
    }
}
