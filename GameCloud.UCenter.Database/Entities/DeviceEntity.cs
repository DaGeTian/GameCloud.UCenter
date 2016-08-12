// Copyright(c) Cragon.All rights reserved.

namespace GameCloud.UCenter
{
    using GameCloud.Database;
    using GameCloud.Database.Attributes;

    /// <summary>
    /// Provide a class for account document.
    /// </summary>
    [CollectionName("Device")]
    public class DeviceEntity : EntityBase
    {
        /// <summary>
        /// Gets or sets the account name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the status
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the super password.
        /// </summary>
        public string OperationSystem { get; set; }
    }
}