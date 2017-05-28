using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using GameCloud.Database.Adapters;
using MongoDB.Driver;

namespace GameCloud.Database
{
    /// <summary>
    /// Provide a class for database context.
    /// </summary>
    [Export]
    public class DatabaseContext
    {
        private readonly ExportProvider exportProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContext" /> class.
        /// </summary>
        /// <param name="exportProvider">Indicating the export provider.</param>
        /// <param name="settings">Indicating the database context settings.</param>
        [ImportingConstructor]
        public DatabaseContext(ExportProvider exportProvider, DatabaseContextSettings settings)
        {
            this.exportProvider = exportProvider;
            this.Settings = settings;
            DatabaseConfig databaseConfig = new DatabaseConfig(settings.ConnectionString, settings.DatabaseName);
            var client = new MongoClient(databaseConfig.MongoClientSettings);
            this.Database = client.GetDatabase(this.Settings.DatabaseName);
        }

        /// <summary>
        /// Gets the database.
        /// </summary>
        public IMongoDatabase Database { get; }

        /// <summary>
        /// Gets the database context settings.
        /// </summary>
        public DatabaseContextSettings Settings { get; }

        public ICollectionAdapter<TEntity> GetAdapter<TEntity>()
            where TEntity : EntityBase
        {
            return new CollectionAdapter<TEntity>(this);
        }
    }
}
