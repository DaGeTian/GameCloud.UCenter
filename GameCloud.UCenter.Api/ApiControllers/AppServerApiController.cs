using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using GameCloud.Database.Adapters;
using GameCloud.UCenter.Common.Models.AppServer;
using GameCloud.UCenter.Common.Portable.Contracts;
using GameCloud.UCenter.Common.Portable.Exceptions;
using GameCloud.UCenter.Common.Portable.Models.AppClient;
using GameCloud.UCenter.Database;
using GameCloud.UCenter.Database.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GameCloud.UCenter.Web.Api.ApiControllers
{
    /// <summary>
    /// UCenter app API controller
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AppServerApiController : ApiControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppServerApiController" /> class.
        /// </summary>
        /// <param name="database">The database context.</param>
        [ImportingConstructor]
        public AppServerApiController(UCenterDatabaseContext database)
            : base(database)
        {
        }

        /// <summary>
        /// Create application.
        /// </summary>
        /// <param name="info">Indicating the application information.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        [HttpPost]
        [Route("api/apps")]
        public async Task<IActionResult> CreateApp([FromBody]AppInfo info, CancellationToken token)
        {
            var app = await this.Database.Apps.GetSingleAsync(info.AppId, token);

            if (app == null)
            {
                // todo: currently, app name equals app id.
                app = new AppEntity
                {
                    Id = info.AppId,
                    Name = info.AppId,
                    AppSecret = info.AppSecret,
                };

                await this.Database.Apps.InsertAsync(app, token);
            }

            var response = new AppResponse
            {
                AppId = info.AppId,
                AppSecret = info.AppSecret
            };
            return this.CreateSuccessResult(response);
        }

        /// <summary>
        /// Create application configuration
        /// </summary>
        /// <param name="info">Indicating the application information.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        [HttpPost]
        [Route("api/apps/configurations")]
        public async Task<IActionResult> CreateAppConfiguration([FromBody]AppConfigurationInfo info, CancellationToken token)
        {
            var appConfiguration = await this.Database.AppConfigurations.GetSingleAsync(info.AppId, token);

            if (appConfiguration == null)
            {
                appConfiguration = new AppConfigurationEntity
                {
                    Id = info.AppId,
                    Configuration = info.Configuration,
                };

                await this.Database.AppConfigurations.InsertAsync(appConfiguration, token);
            }
            else
            {
                await this.Database.AppConfigurations.UpsertAsync(appConfiguration, token);
            }

            var response = new AppConfigurationResponse
            {
                AppId = info.AppId,
                Configuration = info.Configuration
            };
            return this.CreateSuccessResult(response);
        }

        /// <summary>
        /// Get the APP configuration.
        /// </summary>
        /// <param name="appId">Indicating the App id.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        [HttpGet]
        [Route("api/apps/{appId}/configurations")]
        public async Task<IActionResult> GetAppConfiguration(string appId, CancellationToken token)
        {
            var appConfiguration = await this.Database.AppConfigurations.GetSingleAsync(appId, token);

            var response = new AppConfigurationResponse
            {
                AppId = appId,
                Configuration = appConfiguration == null ? string.Empty : appConfiguration.Configuration
            };

            return this.CreateSuccessResult(response);
        }

        /// <summary>
        /// Account logins to app.
        /// </summary>
        /// <param name="appId">Indicating the application id.</param>
        /// <param name="info">Indicating the account information.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        [HttpPost]
        [Route("api/apps/{appId}/accountlogin")]
        public async Task<IActionResult> AccountLogin(string appId, [FromBody]AccountLoginAppInfo info, CancellationToken token)
        {
            var account = await this.GetAndVerifyAccount(info.AccountId, info.AccountToken, token);
            await this.CheckAppPermission(appId, info.AppSecret, token);

            var result = new AccountLoginAppResponse();
            result.AccountId = account.Id;
            result.AccountName = account.AccountName;
            result.AccountToken = account.Token;
            result.LastLoginDateTime = account.LastLoginDateTime;

            var dataId = this.GetAppAccountDataId(info.AppId, info.AccountId);
            var accountData = await this.Database.AppAccountDatas.GetSingleAsync(dataId, token);
            if (accountData != null)
            {
                result.AccountData = accountData.Data;
            }

            return this.CreateSuccessResult(result);
        }

        /// <summary>
        /// Application reads account data.
        /// </summary>
        /// <param name="appId">Indicating the application id.</param>
        /// <param name="info">Indicating the data information.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        [HttpPost]
        [Route("api/apps/{appId}/readdata")]
        public async Task<IActionResult> ReadAccountData(string appId, [FromBody]AppAccountDataInfo info, CancellationToken token)
        {
            await this.CheckAppPermission(appId, info.AppSecret, token);

            var dataId = this.GetAppAccountDataId(info.AppId, info.AccountId);
            var accountData = await this.Database.AppAccountDatas.GetSingleAsync(dataId, token);

            var response = new AppAccountDataResponse
            {
                AppId = info.AppId,
                AccountId = info.AccountId,
                Data = accountData?.Data
            };

            return this.CreateSuccessResult(response);
        }

        /// <summary>
        /// Application reads account data.
        /// </summary>
        /// <param name="appId">Indicating the application id.</param>
        /// <param name="info">Indicating the data information.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        [HttpPost]
        [Route("api/apps/{appId}/writedata")]
        public async Task<IActionResult> WriteAccountData(string appId, [FromBody]AppAccountDataInfo info, CancellationToken token)
        {
            await this.CheckAppPermission(appId, info.AppSecret, token);

            var account = await this.GetAndVerifyAccount(info.AccountId, token);

            var dataId = this.GetAppAccountDataId(info.AppId, info.AccountId);
            var accountData = await this.Database.AppAccountDatas.GetSingleAsync(dataId, token);
            if (accountData != null)
            {
                accountData.Data = info.Data;
            }
            else
            {
                accountData = new AppAccountDataEntity
                {
                    Id = dataId,
                    AppId = info.AppId,
                    AccountId = info.AccountId,
                    Data = info.Data
                };
            }

            await this.Database.AppAccountDatas.UpsertAsync(accountData, token);

            var response = new AppAccountDataResponse
            {
                AppId = info.AppId,
                AccountId = info.AccountId,
                Data = accountData.Data
            };

            return this.CreateSuccessResult(response);
        }

        private async Task CheckAppPermission(string appId, string appSecret, CancellationToken token)
        {
            var app = await this.Database.Apps.GetSingleAsync(appId, token);
            if (app == null)
            {
                throw new UCenterException(UCenterErrorCode.AppNotExists);
            }

            if (app.AppSecret != appSecret)
            {
                throw new UCenterException(UCenterErrorCode.AppTokenUnauthorized);
            }
        }

        private async Task<AccountEntity> GetAndVerifyAccount(string accountId, CancellationToken token)
        {
            var account = await this.Database.Accounts.GetSingleAsync(accountId, token);
            if (account == null)
            {
                throw new UCenterException(UCenterErrorCode.AccountNotExist);
            }

            return account;
        }

        private async Task<AccountEntity> GetAndVerifyAccount(
            string accountId,
            string accountToken,
            CancellationToken token)
        {
            var account = await this.GetAndVerifyAccount(accountId, token);

            if (account.Token != accountToken)
            {
                throw new UCenterException(UCenterErrorCode.AccountTokenUnauthorized);
            }

            return account;
        }

        private string GetAppAccountDataId(string appId, string accountId)
        {
            return $"{appId}_{accountId}";
        }
    }
}