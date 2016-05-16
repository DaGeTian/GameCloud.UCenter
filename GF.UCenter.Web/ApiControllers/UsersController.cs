using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using GF.UCenter.CouchBase;
using GF.UCenter.Web.Models;

namespace GF.UCenter.Web.ApiControllers
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UsersController : ApiControllerBase
    {
        [ImportingConstructor]
        public UsersController(CouchBaseContext db) : base(db)
        {
        }

        public async Task<UserListModel> Get([FromUri]string keyword = null, int page = 0, int count = 1000)
        {
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
            users = users.Skip(page * count).Take(count).ToList();

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