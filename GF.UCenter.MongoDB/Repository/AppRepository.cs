
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace GF.UCenter.MongoDB.Repository
{
    using Database;
    using Entity;

    public interface IAppRepository : IRepository<App> { }

    public class AppRepository : RepositoryBase<App>, IAppRepository
    {
        public AppRepository(IMongoContext context)
            : base(context)
        {
        }

        protected override string GetCollectionName()
        {
            return "App";
        }

        protected override void CreateIndexes(MongoCollection<App> collection)
        {
            base.CreateIndexes(collection);

            collection.EnsureIndex(IndexKeys<App>.Ascending(a => a.Id), IndexOptions.SetName("IX_Id"));
        }
    }
}
