// Copyright(c) Cragon.All rights reserved.

namespace GameCloud.UCenter
{
    using GameCloud.Database;
    using GameCloud.Database.Attributes;

    /// <summary>
    /// Provide a class for application entity.
    /// </summary>
    [CollectionName("AppConfiguration")]
    public class AppConfigurationEntity : EntityBase
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        public string Configuration { get; set; }
    }
}