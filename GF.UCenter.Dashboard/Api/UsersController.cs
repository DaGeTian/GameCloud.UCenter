using System.ComponentModel.Composition;
using System.Linq.Expressions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using GF.UCenter.Common.Settings;
using GF.UCenter.MongoDB;
using GF.UCenter.MongoDB.Adapters;
using GF.UCenter.MongoDB.Entity;
using MongoDB.Driver;
using System;
using GF.UCenter.Web.Common.Modes;

namespace GF.UCenter.Dashboard.Api
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

        public async Task<PaginationResponse<Account>> Get(
            CancellationToken token,
            [FromUri]string keyword = null,
            [FromUri] string orderby = null,
            [FromUri] int page = 1,
            [FromUri] int count = 1000)
        {
            Expression<Func<Account, bool>> filter = null;

            if (!string.IsNullOrEmpty(keyword))
            {
                filter = a => a.AccountName.Contains(keyword)
                    || a.Email.Contains(keyword)
                    || a.PhoneNum.Contains(keyword);
            }

            var total = await this.Database.Accounts.CountAsync(filter, token);

            IQueryable<Account> queryable = this.Database.Accounts.Collection.AsQueryable();
            if (filter != null)
            {
                queryable = queryable.Where(filter);
            }

            var accounts = queryable.Skip((page - 1) * count).Take(count).ToList();

            // todo: add orderby support.
            var model = new PaginationResponse<Account>()
            {
                Page = page,
                PageSize = count,
                Raws = accounts,
                Total = total
            };

            return model;
        }

        public async Task<Account> Get(string id, CancellationToken token)
        {
            var account = await this.Database.Accounts.GetSingleAsync(id, token);

            return account;
        }
    }
}
