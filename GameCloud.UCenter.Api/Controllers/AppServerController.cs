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

namespace GameCloud.UCenter
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AppServerController : BaseController
    {
        //---------------------------------------------------------------------
        //private readonly CacheProvider<AppConfigurationResponse> appConfigurationCacheProvider;
        //private readonly CacheProvider<AppResponse> appCacheProvider;

        //---------------------------------------------------------------------
        [ImportingConstructor]
        public AppServerController(UCenterDatabaseContext database)
            : base(database)
        {
            //this.appConfigurationCacheProvider = new CacheProvider<AppConfigurationResponse>(
            //    TimeSpan.FromMinutes(5),
            //    async (key, token) =>
            //    {
            //        var appConfiguration = await this.Database.AppConfigurations.GetSingleAsync(key, token);

            //        var response = new AppConfigurationResponse
            //        {
            //            AppId = key,
            //            Configuration = appConfiguration == null ? string.Empty : appConfiguration.Configuration
            //        };

            //        return response;
            //    });

            //this.appCacheProvider = new CacheProvider<AppResponse>(
            //    TimeSpan.FromMinutes(5),
            //    async (key, token) =>
            //    {
            //        var app = await this.Database.Apps.GetSingleAsync(key, token);
            //        var response = new AppResponse
            //        {
            //            AppId = key,
            //            AppSecret = app == null ? string.Empty : app.AppSecret,
            //            WechatAppId = app == null ? string.Empty : app.WechatAppId,
            //            WechatAppSecret = app == null ? string.Empty : app.WechatAppSecret
            //        };

            //        return response;
            //    });
        }

        //---------------------------------------------------------------------
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
                    WechatAppId = "",
                    WechatAppSecret = ""
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

        //---------------------------------------------------------------------
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

        //---------------------------------------------------------------------
        [HttpGet]
        [Route("api/apps/{appId}/configurations")]
        public async Task<IActionResult> GetAppConfiguration(string appId, CancellationToken token)
        {
            AppConfigurationResponse response = null;// await this.appConfigurationCacheProvider.Get(appId, token);

            return this.CreateSuccessResult(response);
        }

        //---------------------------------------------------------------------
        [HttpPost]
        [Route("api/apps/{appId}/accountlogin")]
        public async Task<IActionResult> AccountLogin(string appId, [FromBody]AccountLoginAppInfo info, CancellationToken token)
        {
            var account = await this.GetAndVerifyAccount(info.AccountId, info.AccountToken, token);

            this.CheckAppPermission(appId, info.AppSecret);

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

        //---------------------------------------------------------------------
        [HttpPost]
        [Route("api/apps/{appId}/readdata")]
        public async Task<IActionResult> ReadAccountData(string appId, [FromBody]AppAccountDataInfo info, CancellationToken token)
        {
            this.CheckAppPermission(appId, info.AppSecret);

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

        //---------------------------------------------------------------------
        [HttpPost]
        [Route("api/apps/{appId}/writedata")]
        public async Task<IActionResult> WriteAccountData(string appId, [FromBody]AppAccountDataInfo info, CancellationToken token)
        {
            this.CheckAppPermission(appId, info.AppSecret);

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

        //---------------------------------------------------------------------
        private void CheckAppPermission(string appId, string appSecret)
        {
            var app = UCenterContext.Instance.CacheAppEntity.GetAppEntityByAppId(appId);

            if (app == null)
            {
                throw new UCenterException(UCenterErrorCode.AppNotExists);
            }

            if (app.AppSecret != appSecret)
            {
                throw new UCenterException(UCenterErrorCode.AppTokenUnauthorized);
            }
        }

        //---------------------------------------------------------------------
        private async Task<AccountEntity> GetAndVerifyAccount(string accountId, CancellationToken token)
        {
            var account = await this.Database.Accounts.GetSingleAsync(accountId, token);
            if (account == null)
            {
                throw new UCenterException(UCenterErrorCode.AccountNotExist);
            }

            return account;
        }

        //---------------------------------------------------------------------
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

        //---------------------------------------------------------------------
        private string GetAppAccountDataId(string appId, string accountId)
        {
            return $"{appId}_{accountId}";
        }
    }
}
