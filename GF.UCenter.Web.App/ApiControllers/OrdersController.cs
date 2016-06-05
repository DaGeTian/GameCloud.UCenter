namespace GF.UCenter.Web.App.ApiControllers
{
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Common.Modes;
    using global::MongoDB.Driver;
    using MongoDB;
    using MongoDB.Adapters;
    using MongoDB.Entity;
    using UCenter.Common.Settings;

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class OrdersController : ApiControllerBase
    {
        [ImportingConstructor]
        public OrdersController(DatabaseContext db, Settings settings)
            : base(db, settings)
        {
        }

        public async Task<PaginationResponse<OrderEntity>> Get(
            CancellationToken token,
            [FromUri] string accountId = null,
            [FromUri]string keyword = null,
            [FromUri] string orderby = null,
            [FromUri] int page = 1,
            [FromUri] int count = 1000)
        {
            IQueryable<OrderEntity> querable = this.Database.Orders.Collection.AsQueryable();
            if (!string.IsNullOrEmpty(accountId))
            {
                querable = querable.Where(a => a.AccountId == accountId);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                querable = querable.Where(a => a.Id.Contains(keyword)
                    || a.AccountName.Contains(keyword)
                    || a.AppName.Contains(keyword)
                    || a.Content.Contains(keyword));
            }

            var orders = querable.Skip((page - 1) * count).Take(count).ToList();

            var total = querable.LongCount();

            var model = new PaginationResponse<OrderEntity>
            {
                Page = page,
                PageSize = count,
                Total = total,
                Raws = orders
            };

            return model;
        }

        public async Task<OrderRaw> Get(string id, CancellationToken token)
        {
            var order = await this.Database.Orders.GetSingleAsync(id, token);
            var raw = new OrderRaw
            {
                OrderId = order.Id,
                State = order.State,
                AccountId = order.AccountId,
                AppId = order.AppId,
                CompletedTime = order.CompletedTime,
                CreatedTime = order.CreatedTime,
                Content = order.Content
            };

            var account = await this.Database.Accounts.GetSingleAsync(order.AccountId, token);
            raw.AccountName = account.AccountName;

            return raw;
        }
    }
}
