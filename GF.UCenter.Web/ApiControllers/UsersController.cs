using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using GF.UCenter.Common.Settings;
using GF.UCenter.CouchBase;
using GF.UCenter.Web.Models;

namespace GF.UCenter.Web.ApiControllers
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UsersController : ApiControllerBase
    {
        private readonly Settings settings;
        [ImportingConstructor]
        public UsersController(CouchBaseContext db, Settings settings) : base(db)
        {
            this.settings = settings;
        }

        public async Task<UserListModel> Get([FromUri]string keyword = null, [FromUri] string orderby = null, [FromUri] int page = 1, [FromUri] int count = 1000)
        {
            if (page < 1)
            {
                page = 1;
            }
            
            //var abc = this.DatabaseContext.Bucket.Query<object>(queryString);
            IEnumerable<AccountEntity> users;
            Expression<Func<AccountEntity, bool>> expression;
            if (!string.IsNullOrEmpty(keyword))
            {
                expression = a => a.AccountName.Contains(keyword) || a.Email.Contains(keyword) || a.PhoneNum.Contains(keyword);
            }
            else
            {
                expression = null;
            }

            users = await this.DatabaseContext.Bucket.QuerySlimAsync<AccountEntity>(
                    expression,
                    throwIfFailed: false);

            var total = users.Count();
            users = users.Skip((page - 1) * count).Take(count).ToList();

            // todo: add order by support.
            UserListModel model = new UserListModel()
            {
                Count = users.Count(),
                Total = total,
                Page = page,
                Users = users
            };

            return model;
        }

        public async Task<AccountEntity> Get(string id)
        {
            var account = await this.DatabaseContext.Bucket.GetSlimAsync<AccountEntity>(id, throwIfFailed: true);

            return account;
        }
    }
}