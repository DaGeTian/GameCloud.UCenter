using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using GF.Database.Adapters;
using GF.Database.Entity.Common;
using GF.Database.Entity.UCenter;
using MongoDB.Driver;
using TexasPoker.Entity;

namespace GF.Database
{
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
            DatabaseConfig databaseConfig = new DatabaseConfig(settings.ConnectionString, settings.DatabaseName);
            var client = new MongoClient(databaseConfig.MongoClientSettings);
            this.Database = client.GetDatabase(this.Settings.DatabaseName);
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

        /// <summary>
        /// Gets the player.
        /// </summary>
        public ICollectionAdapter<PlayerEntity> Players
        {
            get
            {
                return this.GetAdapter<PlayerEntity>();
            }
        }

        /// <summary>
        /// Gets the event.
        /// </summary>
        public ICollectionAdapter<BuyChipsEventEntity> BuyChipsEvents { get { return this.GetAdapter<BuyChipsEventEntity>(); } }
        public ICollectionAdapter<BuyCoinsEventEntity> BuyCoinsEvents { get { return this.GetAdapter<BuyCoinsEventEntity>(); } }
        public ICollectionAdapter<BuyVIPEventEntity> BuyVIPEvents { get { return this.GetAdapter<BuyVIPEventEntity>(); } }
        public ICollectionAdapter<ChipBuyGiftEventEntity> ChipBuyGiftEvents { get { return this.GetAdapter<ChipBuyGiftEventEntity>(); } }
        public ICollectionAdapter<ChipBuyItemEventEntity> ChipBuyItemEvents { get { return this.GetAdapter<ChipBuyItemEventEntity>(); } }
        public ICollectionAdapter<ChipsChangeByManagerEventEntity> ChipsChangeByManagerEvents { get { return this.GetAdapter<ChipsChangeByManagerEventEntity>(); } }
        public ICollectionAdapter<CoinBuyGiftEventEntity> CoinBuyGiftEvents { get { return this.GetAdapter<CoinBuyGiftEventEntity>(); } }
        public ICollectionAdapter<CoinBuyItemEventEntity> CoinBuyItemEvents { get { return this.GetAdapter<CoinBuyItemEventEntity>(); } }
        public ICollectionAdapter<DailyGetChipsEventEntity> DailyGetChipsEvents { get { return this.GetAdapter<DailyGetChipsEventEntity>(); } }
        public ICollectionAdapter<LostAllSendChipsEventEntity> LostAllSendChipsEvents { get { return this.GetAdapter<LostAllSendChipsEventEntity>(); } }
        public ICollectionAdapter<PlayerGetChipsFromOtherEventEntity> PlayerGetChipsFromOtherEvents { get { return this.GetAdapter<PlayerGetChipsFromOtherEventEntity>(); } }
        public ICollectionAdapter<PlayerReportEventEntity> PlayerReportEvents { get { return this.GetAdapter<PlayerReportEventEntity>(); } }
        public ICollectionAdapter<PlayerSendOtherChipsEventEntity> PlayerSendOtherChipsEvents { get { return this.GetAdapter<PlayerSendOtherChipsEventEntity>(); } }
        public ICollectionAdapter<TexasPokerEventEntity> TexasPokerEvents { get { return this.GetAdapter<TexasPokerEventEntity>(); } }

        public ICollectionAdapter<TEntity> GetAdapter<TEntity>()
            where TEntity : EntityBase
        {
            return this.exportProvider.GetExportedValue<ICollectionAdapter<TEntity>>();
        }
    }
}
