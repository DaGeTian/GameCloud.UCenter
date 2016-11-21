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
    public class DevicesController : ManagerApiControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DevicesController" /> class.
        /// </summary>
        /// <param name="ucenterDb">Indicating the database context.</param>
        /// <param name="ucenterventDb">Indicating the database context.</param>
        /// <param name="settings">Indicating the settings.</param>
        [ImportingConstructor]
        public DevicesController(
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
        [Route("api/manager/devices")]
        public async Task<PluginPaginationResponse<DeviceEntity>> Post([FromBody]SearchRequestInfo request, CancellationToken token)
        {
            string keyword = request.GetParameterValue<string>("keyword");
            int page = request.GetParameterValue<int>("page", 1);
            int count = request.GetParameterValue<int>("pageSize", 10);

            Expression<Func<DeviceEntity, bool>> filter = null;

            if (!string.IsNullOrEmpty(keyword))
            {
                filter = a => a.Id.Contains(keyword);
            }

            var total = await this.UCenterDatabase.Devices.CountAsync(filter, token);

            IQueryable<DeviceEntity> queryable = this.UCenterDatabase.Devices.Collection.AsQueryable();
            if (filter != null)
            {
                queryable = queryable.Where(filter);
            }
            queryable = queryable.OrderByDescending(d => d.CreatedTime);

            var result = queryable.Skip((page - 1) * count).Take(count).ToList();

            // todo: add orderby support.
            var model = new PluginPaginationResponse<DeviceEntity>
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
