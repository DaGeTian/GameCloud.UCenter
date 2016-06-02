
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace GF.UCenter.MongoDB.Repository
{
    using Database;
    using Entity;

    public interface IAccountRepository : IRepository<Account> { }

    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(IMongoContext context)
            : base(context)
        {
        }

        protected override string GetCollectionName()
        {
            return "Account";
        }

        protected override void CreateIndexes(MongoCollection<Account> collection)
        {
            base.CreateIndexes(collection);

            collection.EnsureIndex(IndexKeys<Account>.Ascending(a => a.AccountName), IndexOptions.SetName("IX_AccountName"));
            collection.EnsureIndex(IndexKeys<Account>.Ascending(a => a.Email), IndexOptions.SetName("IX_Email"));
            collection.EnsureIndex(IndexKeys<Account>.Ascending(a => a.PhoneNum), IndexOptions.SetName("IX_PhoneNum"));

        }
    }
}
