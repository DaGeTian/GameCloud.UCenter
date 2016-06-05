namespace GF.UCenter.MongoDB
{
    using System.ComponentModel.Composition;
    using global::MongoDB.Driver;

    [Export]
    public class DatabaseContextSettings
    {
        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }

        // maybe we will need it in the future.
        public MongoCollectionSettings CollectionSettings { get; set; }

        // maybe we will need it in the future.
        public MongoDatabaseSettings DatabaseSettings { get; set; }
    }
}
