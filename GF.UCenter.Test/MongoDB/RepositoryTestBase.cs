using GF.UCenter.MongoDB.Database;

namespace GF.UCenter.Test.MongoDB
{
    using UCenter.MongoDB;

    public class RepositoryTestBase : UCenterE2ETestBase
    {
        protected IMongoContext MongoContext;

        public RepositoryTestBase()
        {
            string url = "mongodb://42.159.239.155:27017/ucenter";
            MongoContext = new MongoContext(url);
        }
    }
}
