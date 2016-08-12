using GameCloud.Database;
using GameCloud.Database.Attributes;

namespace GameCloud.UCenter.Database.Entities
{
    /// <summary>
    /// Provide a class for application entity.
    /// </summary>
    [CollectionName("App")]
    public class AppEntity : EntityBase
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the application secret.
        /// </summary>
        public string AppSecret { get; set; }
    }
}