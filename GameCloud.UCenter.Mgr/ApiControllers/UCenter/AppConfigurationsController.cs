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
    public class AppConfigurationsController : ManagerApiControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfigurationsController" /> class.
        /// </summary>
        /// <param name="ucenterDb">Indicating the database context.</param>
        /// <param name="ucenterventDb">Indicating the database context.</param>
        /// <param name="settings">Indicating the settings.</param>
        [ImportingConstructor]
        public AppConfigurationsController(
            UCenterDatabaseContext ucenterDb,
            UCenterEventDatabaseContext ucenterventDb,
            Settings settings)
            : base(ucenterDb, ucenterventDb, settings)
        {
        }

        /// <summary>
        /// Get app configuration list.
        /// </summary>
        /// <param name="request">Indicating the count.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async return user list.</returns>
        [Route("api/ucenter/appconfigurations")]
        public async Task<PluginPaginationResponse<AppConfigurationEntity>> Post([FromBody]SearchRequestInfo<AppConfigurationEntity> request, CancellationToken token)
        {
            if (request.Method == PluginRequestMethod.Update)
            {
                var updateRawData = request.RawData;
                if (updateRawData != null)
                {
                    var filterDefinition = Builders<AppConfigurationEntity>.Filter.Where(e => e.Id == updateRawData.Id);
                    var updateDefinition = Builders<AppConfigurationEntity>.Update
                        .Set(e => e.Configuration, updateRawData.Configuration);
                    await this.UCenterDatabase.AppConfigurations.UpdateOneAsync(updateRawData, filterDefinition, updateDefinition, token);
                }
            }

            string keyword = request.GetParameterValue<string>("keyword");
            int page = request.GetParameterValue<int>("page", 1);
            int count = request.GetParameterValue<int>("pageSize", 10);

            Expression<Func<AppConfigurationEntity, bool>> filter = null;

            if (!string.IsNullOrEmpty(keyword))
            {
                filter = a => a.Name.Contains(keyword);
            }

            var total = await this.UCenterDatabase.AppConfigurations.CountAsync(filter, token);

            IQueryable<AppConfigurationEntity> queryable = this.UCenterDatabase.AppConfigurations.Collection.AsQueryable();
            if (filter != null)
            {
                queryable = queryable.Where(filter);
            }

            var result = queryable.Skip((page - 1) * count).Take(count).ToList();

            // todo: add orderby support.
            var model = new PluginPaginationResponse<AppConfigurationEntity>
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
