namespace GF.Manager.TexasPoker.ApiControllers
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using UCenter.Common.Settings;
    using UCenter.MongoDB;
    using UCenter.MongoDB.Adapters;
    using UCenter.Web.Common.Modes;
    using MongoDB.Driver;
    using UCenter.MongoDB.TexasPoker;

    /// <summary>
    /// Provide a controller for events.
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/events")]
    public class PlayerSendOtherChipsEventsController : ApiControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventsController" /> class.
        /// </summary>
        /// <param name="database">Indicating the database context.</param>
        /// <param name="settings">Indicating the settings.</param>
        [ImportingConstructor]
        public PlayerSendOtherChipsEventsController(DatabaseContext database, Settings settings)
            : base(database, settings)
        {
        }

        /// <summary>
        /// Get event list.
        /// </summary>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <param name="keyword">Indicating the keyword.</param>
        /// <param name="orderby">Indicating the order by name.</param>
        /// <param name="page">Indicating the page number.</param>
        /// <param name="count">Indicating the count.</param>
        /// <returns>Async return event list.</returns>
        [Route("PlayerSendOtherChips")]
        public async Task<PaginationResponse<PlayerSendOtherChipsEventEntity>> Get(
            CancellationToken token,
            [FromUri] string keyword = null,
            [FromUri] string orderby = null,
            [FromUri] int page = 1,
            [FromUri] int count = 1000)
        {
            Expression<Func<PlayerSendOtherChipsEventEntity, bool>> filter = null;

            if (!string.IsNullOrEmpty(keyword))
            {
                filter = e => e.SendPlayerEtGuid == keyword
                    || e.TargetPlayerEtGuid == keyword;
            }

            var total = await this.Database.PlayerSendOtherChipsEvents.CountAsync(filter, token);

            IQueryable<PlayerSendOtherChipsEventEntity> queryable = this.Database.PlayerSendOtherChipsEvents.Collection.AsQueryable();
            if (filter != null)
            {
                queryable = queryable.Where(filter);
            }

            var result = queryable.Skip((page - 1) * count).Take(count).ToList();

            // todo: add orderby support.
            var model = new PaginationResponse<PlayerSendOtherChipsEventEntity>
            {
                Page = page,
                PageSize = count,
                Raws = result,
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
        public async Task<PlayerSendOtherChipsEventEntity> Get(string id, CancellationToken token)
        {
            var result = await this.Database.PlayerSendOtherChipsEvents.GetSingleAsync(id, token);

            return result;
        }
    }
}
