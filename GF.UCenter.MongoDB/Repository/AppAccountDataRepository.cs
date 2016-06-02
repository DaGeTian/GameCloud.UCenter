
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace GF.UCenter.MongoDB.Repository
{
    using Database;
    using Entity;

    public interface IAppAccountDataRepository : IRepository<AppAccountData> { }

    public class AppAccountDataRepository : RepositoryBase<AppAccountData>, IAppAccountDataRepository
    {
        public AppAccountDataRepository(IMongoContext context)
            : base(context)
        {
        }

        protected override string GetCollectionName()
        {
            return "AppAccountData";
        }

        protected override void CreateIndexes(MongoCollection<AppAccountData> collection)
        {
            base.CreateIndexes(collection);

            collection.EnsureIndex(IndexKeys<AppAccountData>.Ascending(a => a.Id), IndexOptions.SetName("IX_Id"));
        }
    }
}
