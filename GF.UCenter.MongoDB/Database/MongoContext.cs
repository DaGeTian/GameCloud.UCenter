using MongoDB.Driver;

namespace GF.UCenter.MongoDB.Database
{
    public class MongoContext : IMongoContext
    {
        private readonly IMongoDatabase database;

        public MongoContext(string url)
        {
            string dbName = url.Substring(url.LastIndexOf("/") + 1);
            var client = new MongoClient(url);
            database = client.GetDatabase(dbName);
        }

        public IMongoDatabase GetDatabase()
        {
            return database;
        }
    }


}