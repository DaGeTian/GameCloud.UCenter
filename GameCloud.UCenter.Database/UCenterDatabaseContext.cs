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
        /// Gets the device record adapter.
        /// </summary>
        public ICollectionAdapter<DeviceEntity> Devices
        {
            get
            {
                return this.GetAdapter<DeviceEntity>();
            }
        }

        /// <summary>
        /// Gets the device record adapter.
        /// </summary>
        public ICollectionAdapter<GuestDeviceEntity> GuestDevices
        {
            get
            {
                return this.GetAdapter<GuestDeviceEntity>();
            }
        }

        /// <summary>
        /// Gets the login record adapter.
        /// </summary>
        public ICollectionAdapter<ErrorEventEntity> ErrorEvents
        {
            get
            {
                return this.GetAdapter<ErrorEventEntity>();
            }
        }

        /// <summary>
        /// Gets the login record adapter.
        /// </summary>
        public ICollectionAdapter<AccountEventEntity> AccountEvents
        {
            get
            {
                return this.GetAdapter<AccountEventEntity>();
            }
        }
    }
}
