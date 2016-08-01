using GF.Database.Entity.Common;
using GF.Database.Entity.Common.Attributes;

namespace GF.Database.Entity.UCenter
{
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