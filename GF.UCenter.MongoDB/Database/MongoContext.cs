using MongoDB.Driver;

namespace GF.UCenter.MongoDB.Database
{
    public class MongoContext : IMongoContext
    {
        private readonly MongoDatabase database;

        public MongoContext(string url)
        {
            string dbName = url.Substring(url.LastIndexOf("/") + 1);
            var client = new MongoClient(url);
            // todo: use new API
            database = client.GetServer().GetDatabase(dbName);
        }

        public MongoDatabase GetDatabase()
        {
            return database;
        }
    }


}