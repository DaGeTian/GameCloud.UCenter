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
    public class ExceptionEventsController : ManagerApiControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorEventsController" /> class.
        /// </summary>
        /// <param name="ucenterDb">Indicating the database context.</param>
        /// <param name="ucenterventDb">Indicating the database context.</param>
        /// <param name="settings">Indicating the settings.</param>
        [ImportingConstructor]
        public ExceptionEventsController(
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
        [Route("api/manager/exceptionEvents")]
        public async Task<PluginPaginationResponse<ExceptionEventEntity>> Post([FromBody]SearchRequestInfo<ExceptionEventEntity> request, CancellationToken token)
        {
            if (request.Method == PluginRequestMethod.Delete)
            {
                var deleteRawData = request.RawData;
                if (deleteRawData != null)
                {
                    await this.UCenterEventDatabase.ExceptionEvents.DeleteAsync(
                        v => v.Id == deleteRawData.Id, token);
                }
            }

            string keyword = request.GetParameterValue<string>("keyword");
            int page = request.GetParameterValue<int>("page", 1);
            int count = request.GetParameterValue<int>("pageSize", 10);

            Expression<Func<ExceptionEventEntity, bool>> filter = null;

            if (!string.IsNullOrEmpty(keyword))
            {
                filter = a => a.Message.Contains(keyword);
            }

            var total = await this.UCenterEventDatabase.ExceptionEvents.CountAsync(filter, token);

            IQueryable<ExceptionEventEntity> queryable = this.UCenterEventDatabase.ExceptionEvents.Collection.AsQueryable();
            if (filter != null)
            {
                queryable = queryable.Where(filter);
            }
            queryable = queryable.OrderByDescending(e => e.CreatedTime);

            var result = queryable.Skip((page - 1) * count).Take(count).ToList();

            // todo: add orderby support.
            var model = new PluginPaginationResponse<ExceptionEventEntity>
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
