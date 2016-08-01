using GF.Database.Adapters;
using MongoDB.Driver;

namespace GF.UCenter.Manager.ApiControllers
{
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using GF.UCenter.Common.Settings;
    using GF.UCenter.Database;
    using GF.UCenter.Web.Common.Modes;
    /// <summary>
    /// Provide an order controller.
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class OrdersController : ApiControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrdersController" /> class.
        /// </summary>
        /// <param name="db">Indicating the database context.</param>
        /// <param name="settings">Indicating the settings.</param>
        [ImportingConstructor]
        public OrdersController(UCenterDatabaseContext db, Settings settings)
            : base(db, settings)
        {
        }

        /// <summary>
        /// Get order list
        /// </summary>
        /// <param name="accountId">Indicating the user account id.</param>
        /// <param name="keyword">Indicating the keyword.</param>
        /// <param name="orderby">Indicating the order by name.</param>
        /// <param name="page">Indicating the page number.</param>
        /// <param name="count">Indicating the count.</param>
        /// <returns>Async return order list.</returns>
        public PaginationResponse<OrderEntity> Get(
            [FromUri] string accountId = null,
            [FromUri] string keyword = null,
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

        /// <summary>
        /// Get single order detail.
        /// </summary>
        /// <param name="id">Indicating the order id.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        public async Task<OrderEntity> Get(string id, CancellationToken token)
        {
            var result = await this.Database.Orders.GetSingleAsync(id, token);

            return result;
        }
    }
}
