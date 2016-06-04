namespace GF.UCenter.MongoDB.Database
{
    using System;
    using global::MongoDB.Driver;

    public class MongoContext : IMongoContext
    {
        private readonly IMongoDatabase database;

        public MongoContext(string url)
        {
            string dbName = url.Substring(url.LastIndexOf("/", StringComparison.InvariantCultureIgnoreCase) + 1);
            var client = new MongoClient(url);
            database = client.GetDatabase(dbName);
        }

        public IMongoDatabase GetDatabase()
        {
            return database;
        }
    }


}