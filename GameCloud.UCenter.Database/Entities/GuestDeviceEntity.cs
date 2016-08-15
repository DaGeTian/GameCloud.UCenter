using GameCloud.Database;
using GameCloud.Database.Attributes;
using GameCloud.UCenter.Common.Portable.Models.AppClient;

namespace GameCloud.UCenter.Database.Entities
{
    [CollectionName("GuestDevice")]
    public class GuestDeviceEntity : EntityBase
    {
        /// <summary>
        /// Gets or sets the account id.
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Gets or sets the app id
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// Gets or sets the device id.
        /// </summary>
        public DeviceInfo Device { get; set; }
    }
}
