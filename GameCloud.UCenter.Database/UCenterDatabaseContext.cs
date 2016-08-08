using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using GameCloud.Database;
using GameCloud.Database.Adapters;
using GameCloud.UCenter.Database.Entities;

namespace GameCloud.UCenter.Database
{
    [Export]
    public class UCenterDatabaseContext : DatabaseContext
    {
        [ImportingConstructor]
        public UCenterDatabaseContext(ExportProvider exportProvider, DatabaseContextSettings settings)
            : base(exportProvider, settings)
        {
        }

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
    }
}
