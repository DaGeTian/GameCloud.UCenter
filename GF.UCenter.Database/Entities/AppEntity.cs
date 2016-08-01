using GF.Database;
using GF.Database.Attributes;

namespace GF.UCenter.Database
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