using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using GameCloud.Database.Adapters;
using GameCloud.Manager.PluginContract.Requests;
using GameCloud.Manager.PluginContract.Responses;
using GameCloud.UCenter.Common.Settings;
using GameCloud.UCenter.Database;
using GameCloud.UCenter.Database.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace GameCloud.UCenter.Api.Manager.ApiControllers
{
    /// <summary>
    /// Provide a controller for users.
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AccountErrorEventsController : ManagerApiControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorEventsController" /> class.
        /// </summary>
        /// <param name="ucenterDb">Indicating the database context.</param>
        /// <param name="ucenterventDb">Indicating the database context.</param>
        /// <param name="settings">Indicating the settings.</param>
        [ImportingConstructor]
        public AccountErrorEventsController(
            UCenterDatabaseContext ucenterDb,
            UCenterEventDatabaseContext ucenterventDb,
            Settings settings)
            : base(ucenterDb, ucenterventDb, settings)
        {
        }

        /// <summary>
        /// Get user list.
        /// </summary>
        /// <param name="request">Indicating the count.</param>
        /// <returns>Async return user list.</returns>
        [Route("api/manager/accountErrorEvents")]
        public async Task<PluginPaginationResponse<AccountErrorEventEntity>> Post([FromBody]SearchRequestInfo<AccountErrorEventEntity> request, CancellationToken token)
        {
            string keyword = request.GetParameterValue<string>("keyword");
            int page = request.GetParameterValue<int>("page", 1);
            int count = request.GetParameterValue<int>("pageSize", 10);

            Expression<Func<AccountErrorEventEntity, bool>> filter = null;

            if (!string.IsNullOrEmpty(keyword))
            {
                filter = a => a.AccountName.Contains(keyword);
            }

            var total = await this.UCenterEventDatabase.AccountErrorEvents.CountAsync(filter, token);

            IQueryable<AccountErrorEventEntity> queryable = this.UCenterEventDatabase.AccountErrorEvents.Collection.AsQueryable();
            if (filter != null)
            {
                queryable = queryable.Where(filter);
            }
            queryable = queryable.OrderByDescending(e => e.CreatedTime);

            var result = queryable.Skip((page - 1) * count).Take(count).ToList();

            // todo: add orderby support.
            var model = new PluginPaginationResponse<AccountErrorEventEntity>
            {
                Page = page,
                PageSize = count,
                Raws = result,
                Total = total
            };

            return model;
        }
    }
}
