namespace GF.UCenter.MongoDB
{
    using System.ComponentModel.Composition;
    using global::MongoDB.Driver;

    /// <summary>
    /// Provide a class for database context settings.
    /// </summary>
    [Export]
    public class DatabaseContextSettings
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the collection settings.
        /// </summary>
        public MongoCollectionSettings CollectionSettings { get; set; }

        /// <summary>
        /// Gets or sets the database settings.
        /// </summary>
        public MongoDatabaseSettings DatabaseSettings { get; set; }
    }
}
