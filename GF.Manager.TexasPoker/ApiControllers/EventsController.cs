namespace GF.Manager.TexasPoker.ApiControllers
{
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
    /// <summary>
    /// Provide a controller for users.
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/events")]
    public class EventsController : ApiControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventsController" /> class.
        /// </summary>
        /// <param name="database">Indicating the database context.</param>
        /// <param name="settings">Indicating the settings.</param>
        [ImportingConstructor]
        public EventsController(DatabaseContext database, Settings settings)
            : base(database, settings)
        {
        }

        /// <summary>
        /// Get resouece list.
        /// </summary>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <param name="keyword">Indicating the keyword.</param>
        /// <param name="orderby">Indicating the order by name.</param>
        /// <param name="page">Indicating the page number.</param>
        /// <param name="count">Indicating the count.</param>
        /// <returns>Async return resouece list.</returns>
        public async Task<PaginationResponse<EventEntity>> Get(
            CancellationToken token,
            [FromUri]string keyword = null,
            [FromUri] string orderby = null,
            [FromUri] int page = 1,
            [FromUri] int count = 1000)
        {
            Expression<Func<EventEntity, bool>> filter = null;

            if (!string.IsNullOrEmpty(keyword))
            {
                filter = a => a.GameSpeed.Contains(keyword);

            }

            var total = await this.Database.Events.CountAsync(filter, token);

            IQueryable<EventEntity> queryable = this.Database.Events.Collection.AsQueryable();
            if (filter != null)
            {
                queryable = queryable.Where(filter);
            }

            var events = queryable.Skip((page - 1) * count).Take(count).ToList();

            // todo: add orderby support.
            var model = new PaginationResponse<EventEntity>
            {
                Page = page,
                PageSize = count,
                Raws = events,
                Total = total
            };

            return model;
        }

        /// <summary>
        /// Get single user details.
        /// </summary>
        /// <param name="id">Indicating the user id.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async return user details.</returns>
        public async Task<EventEntity> Get(string id, CancellationToken token)
        {
            var result = await this.Database.Events.GetSingleAsync(id, token);

            return result;
        }
    }
}
