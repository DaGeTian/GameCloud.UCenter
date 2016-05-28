using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Web.Http;
using GF.UCenter.Common.Settings;
using GF.UCenter.CouchBase;
using GF.UCenter.CouchBase.Models;

namespace GF.UCenter.Dashboard.Api
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UsersController : ApiControllerBase
    {
        [ImportingConstructor]
        public UsersController(CouchBaseContext database, Settings settings)
            : base(database, settings)
        {
        }

        public async Task<PaginationResponse<AccountEntity>> Get([FromUri]string keyword = null, [FromUri] string orderby = null, [FromUri] int page = 1, [FromUri] int count = 1000)
        {
            var expression = new PaginationQueryExpression<AccountEntity>(page, count);
            if (!string.IsNullOrEmpty(keyword))
            {
                expression.AddLikeItem(e => e.AccountName, keyword);
                expression.AddLikeItem(e => e.Email, keyword);
                expression.AddLikeItem(e => e.PhoneNum, keyword);
                expression.AddLikeItem(e => e.IdentityNum, keyword);
            }

            if (!string.IsNullOrEmpty(orderby))
            {
                expression.AddOrderByItem(orderby, OrderByType.ASC);
            }

            var model = await this.Database.Bucket.PaginationQuerySlimAsync<AccountEntity>(expression);

            return model;
        }

        public async Task<AccountEntity> Get(string id)
        {
            var account = await this.Database.Bucket.GetSlimAsync<AccountEntity>(id, throwIfFailed: true);

            return account;
        }
    }
}
