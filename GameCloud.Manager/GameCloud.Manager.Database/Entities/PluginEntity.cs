using System;
using GameCloud.Database;
using GameCloud.Database.Attributes;

namespace GameCloud.Manager.Database.Entities
{
    /// <summary>
    /// Provide a class for account document.
    /// </summary>
    [CollectionName("Plugin")]
    public class PluginEntity : EntityBase
    {
        /// <summary>
        /// Gets or sets the plugin name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the plugin display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the plugin description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the plugin url.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the plugin certification.
        /// </summary>
        public string Certificate { get; set; }
    }
}