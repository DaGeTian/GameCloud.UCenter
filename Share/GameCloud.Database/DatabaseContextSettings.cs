using System.ComponentModel.Composition;
using MongoDB.Driver;

namespace GameCloud.Database
{
    /// <summary>
    /// Provide a class for database context settings.
    /// </summary>
    [Export]
    public class DatabaseContextSettings
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        public virtual string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        public virtual string DatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the collection settings.
        /// </summary>
        public virtual MongoCollectionSettings CollectionSettings { get; set; }

        /// <summary>
        /// Gets or sets the database settings.
        /// </summary>
        public virtual MongoDatabaseSettings DatabaseSettings { get; set; }
    }
}
