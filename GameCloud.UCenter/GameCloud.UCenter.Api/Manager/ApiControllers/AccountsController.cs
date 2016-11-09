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
    public class AccountsController : ManagerApiControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountsController" /> class.
        /// </summary>
        /// <param name="ucenterDb">Indicating the database context.</param>
        /// <param name="ucenterventDb">Indicating the database context.</param>
        /// <param name="settings">Indicating the settings.</param>
        [ImportingConstructor]
        public AccountsController(
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
        [Route("api/manager/accounts")]
        public async Task<PluginPaginationResponse<AccountEntity>> Post([FromBody]SearchRequestInfo<AccountEntity> request, CancellationToken token)
        {
            if (request.Method == PluginRequestMethod.Update)
            {
                var updateRawData = request.RawData;
                if (updateRawData != null)
                {
                    // todo: update the other properties here.
                    var updateFilter = Builders<AccountEntity>.Filter.Where(e => e.Id == updateRawData.Id);
                    var updateDefinition = Builders<AccountEntity>.Update.Set(e => e.AccountStatus, updateRawData.AccountStatus);
                    await this.UCenterDatabase.Accounts.UpdateOneAsync(updateRawData, updateFilter, updateDefinition, token);
                }
            }

            string keyword = request.Keyword;
            int page = request.Page;
            int count = request.PageSize;

            Expression<Func<AccountEntity, bool>> filter = null;

            if (!string.IsNullOrEmpty(keyword))
            {
                filter = a => a.AccountName.Contains(keyword)
                    || a.Email.Contains(keyword)
                    || a.Phone.Contains(keyword);
            }

            var total = await this.UCenterDatabase.Accounts.CountAsync(filter, token);

            IQueryable<AccountEntity> queryable = this.UCenterDatabase.Accounts.Collection.AsQueryable();
            if (filter != null)
            {
                queryable = queryable.Where(filter);
            }
            queryable = queryable.OrderByDescending(a => a.CreatedTime);

            var result = queryable.Skip((page - 1) * count).Take(count).ToList();

            // todo: add orderby support.
            var model = new PluginPaginationResponse<AccountEntity>
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
