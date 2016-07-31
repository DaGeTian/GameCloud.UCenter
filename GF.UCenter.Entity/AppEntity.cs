using GF.MongoDB.Common;
using GF.MongoDB.Common.Attributes;

namespace GF.UCenter.Entity
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