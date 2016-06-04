using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using GF.UCenter.Common.Settings;
using GF.UCenter.MongoDB;
using GF.UCenter.MongoDB.Adapters;
using GF.UCenter.MongoDB.Entity;
using GF.UCenter.Web.Common.Modes;
using MongoDB.Driver;

namespace GF.UCenter.Web.App.ApiControllers
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UsersController : ApiControllerBase
    {
        [ImportingConstructor]
        public UsersController(DatabaseContext database, Settings settings)
            : base(database, settings)
        {
        }

        public async Task<PaginationResponse<AccountEntity>> Get(
            CancellationToken token,
            [FromUri]string keyword = null,
            [FromUri] string orderby = null,
            [FromUri] int page = 1,
            [FromUri] int count = 1000)
        {
            Expression<Func<AccountEntity, bool>> filter = null;

            if (!string.IsNullOrEmpty(keyword))
            {
                filter = a => a.AccountName.Contains(keyword)
                    || a.Email.Contains(keyword)
                    || a.PhoneNum.Contains(keyword);
            }

            var total = await this.Database.Accounts.CountAsync(filter, token);

            IQueryable<AccountEntity> queryable = this.Database.Accounts.Collection.AsQueryable();
            if (filter != null)
            {
                queryable = queryable.Where(filter);
            }

            var accounts = queryable.Skip((page - 1) * count).Take(count).ToList();

            // todo: add orderby support.
            var model = new PaginationResponse<AccountEntity>()
            {
                Page = page,
                PageSize = count,
                Raws = accounts,
                Total = total
            };

            return model;
        }

        public async Task<AccountEntity> Get(string id, CancellationToken token)
        {
            var account = await this.Database.Accounts.GetSingleAsync(id, token);

            return account;
        }
    }
}
