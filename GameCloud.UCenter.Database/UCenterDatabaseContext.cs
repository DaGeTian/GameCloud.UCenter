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
        /// Gets the account collection adapter.
        /// </summary>
        public ICollectionAdapter<AccountEntity> Accounts
        {
            get
            {
                return this.GetAdapter<AccountEntity>();
            }
        }

        /// <summary>
        /// Gets the app configuration collection adapter.
        /// </summary>
        public ICollectionAdapter<AppConfigurationEntity> AppConfigurations
        {
            get
            {
                return this.GetAdapter<AppConfigurationEntity>();
            }
        }

        /// <summary>
        /// Gets the application collection adapter.
        /// </summary>
        public ICollectionAdapter<AppEntity> Apps
        {
            get
            {
                return this.GetAdapter<AppEntity>();
            }
        }

        /// <summary>
        /// Gets the account application data collection adapter.
        /// </summary>
        public ICollectionAdapter<AppAccountDataEntity> AppAccountDatas
        {
            get
            {
                return this.GetAdapter<AppAccountDataEntity>();
            }
        }

        /// <summary>
        /// Gets the key placeholder collection adapter.
        /// </summary>
        public ICollectionAdapter<KeyPlaceholderEntity> KeyPlaceholders
        {
            get
            {
                return this.GetAdapter<KeyPlaceholderEntity>();
            }
        }

        /// <summary>
        /// Gets the device collection adapter.
        /// </summary>
        public ICollectionAdapter<DeviceEntity> Devices
        {
            get
            {
                return this.GetAdapter<DeviceEntity>();
            }
        }

        /// <summary>
        /// Gets the guest device collection adapter.
        /// </summary>
        public ICollectionAdapter<GuestDeviceEntity> GuestDevices
        {
            get
            {
                return this.GetAdapter<GuestDeviceEntity>();
            }
        }

        /// <summary>
        /// Gets the order collection adapter.
        /// </summary>
        public ICollectionAdapter<OrderEntity> Orders
        {
            get
            {
                return this.GetAdapter<OrderEntity>();
            }
        }
    }
}
