using GF.Database;
using GF.Database.Adapters;
using GF.Database.Entity.UCenter;

namespace GF.UCenter.Web.Api.ApiControllers
{
    using System;
    using System.ComponentModel.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Common.Logger;
    using UCenter.Common;
    using UCenter.Common.Portable.Contracts;
    using UCenter.Common.Portable.Exceptions;

    /// <summary>
    /// UCenter app API controller
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/app")]
    public class AppServerApiController : ApiControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppServerApiController" /> class.
        /// </summary>
        /// <param name="database">The database context.</param>
        [ImportingConstructor]
        public AppServerApiController(DatabaseContext database)
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
        [Route("create")]
        public async Task<IHttpActionResult> CreateApp([FromBody] AppInfo info, CancellationToken token)
        {
            CustomTrace.TraceInformation("[CreateApp] AppId={0}", info.AppId);

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
        /// Verify the account.
        /// </summary>
        /// <param name="info">Indicating the account information.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        [HttpPost]
        [Route("verifyaccount")]
        public async Task<IHttpActionResult> VerifyAccount(AppVerifyAccountInfo info, CancellationToken token)
        {
            CustomTrace.TraceInformation($"App.VerifyAccount AppId={info.AppId} AccountId={info.AccountId}");

            await this.VerifyApp(info.AppId, info.AppSecret, token);
            var account = await this.GetAndVerifyAccount(info.AccountId, info.AccountToken, token);

            var result = new AppVerifyAccountResponse();
            result.AccountId = account.Id;
            result.AccountName = account.AccountName;
            result.AccountToken = account.Token;
            result.LastLoginDateTime = account.LastLoginDateTime;
            result.LastVerifyDateTime = DateTime.UtcNow;

            return this.CreateSuccessResult(result);
        }

        /// <summary>
        /// Read application account data.
        /// </summary>
        /// <param name="info">Indicating the data information.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        [HttpPost]
        [Route("readdata")]
        public async Task<IHttpActionResult> ReadAppAccountData(AppAccountDataInfo info, CancellationToken token)
        {
            CustomTrace.TraceInformation($"App.ReadAppAccountData AppId={info.AppId} AccountId={info.AccountId}");

            await this.VerifyApp(info.AppId, info.AppSecret, token);

            var account = await this.GetAndVerifyAccount(info.AccountId, token);
            var dataId = this.CreateAppAccountDataId(info.AppId, info.AccountId);
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
        /// Write application account data.
        /// </summary>
        /// <param name="info">Indicating the data information.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        [HttpPost]
        [Route("writedata")]
        public async Task<IHttpActionResult> WriteAppAccountData(AppAccountDataInfo info, CancellationToken token)
        {
            CustomTrace.TraceInformation($"App.WriteAppAccountData AppId={info.AppId} AccountId={info.AccountId}");

            await this.VerifyApp(info.AppId, info.AppSecret, token);

            var account = await this.GetAndVerifyAccount(info.AccountId, token);

            var dataId = this.CreateAppAccountDataId(info.AppId, info.AccountId);
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

        private async Task VerifyApp(string appId, string appSecret, CancellationToken token)
        {
            var app = await this.Database.Apps.GetSingleAsync(appId, token);
            if (app == null)
            {
                throw new UCenterException(UCenterErrorCode.AppNotExit);
            }

            if (appSecret != app.AppSecret)
            {
                throw new UCenterException(UCenterErrorCode.AppAuthFailedSecretNotMatch);
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
                throw new UCenterException(UCenterErrorCode.AccountLoginFailedTokenNotMatch);
            }

            return account;
        }

        private string CreateAppAccountDataId(string appId, string accountId)
        {
            return $"{appId}##{accountId}";
        }
    }
}