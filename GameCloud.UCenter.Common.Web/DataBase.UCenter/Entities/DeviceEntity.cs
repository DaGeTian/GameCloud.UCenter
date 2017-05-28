using GameCloud.Database;
using GameCloud.Database.Attributes;

namespace GameCloud.UCenter.Database.Entities
{
    /// <summary>
    /// Provide a class for account document.
    /// </summary>
    [CollectionName("Device")]
    public class DeviceEntity : EntityBase
    {
        /// <summary>
        /// Gets or sets the device name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the device type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the device model.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the device operation system.
        /// </summary>
        public string OperationSystem { get; set; }
    }
}