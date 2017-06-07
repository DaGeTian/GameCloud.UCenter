using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using GameCloud.Database.Adapters;
using GameCloud.UCenter.Common;
using GameCloud.UCenter.Common.Extensions;
using GameCloud.UCenter.Common.Models.AppClient;
using GameCloud.UCenter.Common.Portable.Contracts;
using GameCloud.UCenter.Common.Portable.Exceptions;
using GameCloud.UCenter.Common.Portable.Models.AppClient;
using GameCloud.UCenter.Common.Settings;
using GameCloud.UCenter.Database;
using GameCloud.UCenter.Database.Entities;
using GameCloud.UCenter.Web.Common.Logger;
using GameCloud.UCenter.Web.Common.Storage;

namespace GameCloud.UCenter
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AccountApiController : ApiControllerBase
    {
        //---------------------------------------------------------------------
        private readonly Settings settings;
        private readonly IStorageContext storageContext;
        private readonly EventTrace eventTrace;
        private readonly ILogger logger;
        private readonly WebClient webClient;

        //---------------------------------------------------------------------
        [ImportingConstructor]
        public AccountApiController(
            UCenterDatabaseContext database,
            Settings settings,
            IStorageContext storageContext,
            EventTrace eventTrace,
            ILoggerFactory logger_factory)
            : base(database)
        {
            this.settings = settings;
            this.storageContext = storageContext;
            this.eventTrace = eventTrace;
            this.logger = logger_factory.CreateLogger("Default");
            this.webClient = new WebClient();
        }

        //---------------------------------------------------------------------
        [HttpPost]
        [Route("api/accounts/register")]
        public async Task<IActionResult> Register([FromBody]AccountRegisterRequestInfo info, CancellationToken token)
        {
            // 检测注册信息合法性
            ValidateAccount(info);

            try
            {
                // 检测帐号名是否可以被注册
                var oldAccountEntity = await this.Database.Accounts.GetSingleAsync(
                    a => a.AccountName == info.AccountName,
                    //|| a.Email == info.Email
                    //|| a.Phone == info.Phone,
                    token);
                if (oldAccountEntity != null)
                {
                    throw new UCenterException(UCenterErrorCode.AccountNameAlreadyExist);
                }

                // 检查Device是否绑定了游客号
                AccountEntity accountEntity = null;
                if (info != null
                    && !string.IsNullOrEmpty(info.AppId)
                    && info.Device != null
                    && !string.IsNullOrEmpty(info.Device.Id))
                {
                    string guestDeviceId = $"{info.AppId}_{info.Device.Id}";
                    var guestDeviceEntity = await this.Database.GuestDevices.GetSingleAsync(guestDeviceId, token);
                    if (guestDeviceEntity != null)
                    {
                        accountEntity = await this.Database.Accounts.GetSingleAsync(guestDeviceEntity.AccountId, token);
                    }
                }

                bool guestConvert = true;
                if (accountEntity == null)
                {
                    guestConvert = false;

                    // 没有绑定游客号，正常注册
                    accountEntity = new AccountEntity
                    {
                        Id = Guid.NewGuid().ToString(),
                    };
                }

                accountEntity.AccountName = info.AccountName;
                accountEntity.AccountType = AccountType.NormalAccount;
                accountEntity.AccountStatus = AccountStatus.Active;
                accountEntity.Name = info.Name;
                accountEntity.Identity = info.Identity;
                accountEntity.Password = EncryptHelper.ComputeHash(info.Password);
                accountEntity.SuperPassword = EncryptHelper.ComputeHash(info.SuperPassword);
                accountEntity.Phone = info.Phone;
                accountEntity.Email = info.Email;
                accountEntity.Gender = info.Gender;

                //var placeholders = new[]
                //    {
                //        this.GenerateKeyPlaceholder(accountEntity.AccountName, KeyType.Name, accountEntity.Id, accountEntity.AccountName),
                //        this.GenerateKeyPlaceholder(accountEntity.Phone, KeyType.Phone, accountEntity.Id, accountEntity.AccountName),
                //        this.GenerateKeyPlaceholder(accountEntity.Email, KeyType.Email, accountEntity.Id, accountEntity.AccountName)
                //    };

                //foreach (var placeholder in placeholders)
                //{
                //    if (!string.IsNullOrEmpty(placeholder.Name))
                //    {
                //        await this.Database.KeyPlaceholders.InsertAsync(placeholder, token);
                //        removeTempsIfError.Add(placeholder);
                //    }
                //}

                if (guestConvert)
                {
                    // 绑定了游客号，游客号转正
                    await this.Database.Accounts.UpsertAsync(accountEntity, token);

                    try
                    {
                        await this.Database.GuestDevices.DeleteAsync(
                            d => d.AppId == info.AppId && d.AccountId == accountEntity.Id,
                            token);
                    }
                    catch (Exception ex)
                    {
                        //CustomTrace.TraceError(ex, "Error to remove guest device");
                    }

                    await this.TraceAccountEvent(accountEntity, "GuestConvert", token: token);
                }
                else
                {
                    // 没有绑定游客号，正常注册

                    // Remove this from UCenter for performance concern
                    // If user does not have default profile icon, app client should use local default one
                    // accountEntity.ProfileImage = await this.storageContext.CopyBlobAsync(
                    //    accountEntity.Gender == Gender.Female ? this.settings.DefaultProfileImageForFemaleBlobName : this.settings.DefaultProfileImageForMaleBlobName,
                    //    this.settings.ProfileImageForBlobNameTemplate.FormatInvariant(accountEntity.Id),
                    //    token);

                    // accountEntity.ProfileThumbnail = await this.storageContext.CopyBlobAsync(
                    //    accountEntity.Gender == Gender.Female
                    //        ? this.settings.DefaultProfileThumbnailForFemaleBlobName
                    //        : this.settings.DefaultProfileThumbnailForMaleBlobName,
                    //    this.settings.ProfileThumbnailForBlobNameTemplate.FormatInvariant(accountEntity.Id),
                    //    token);

                    await this.Database.Accounts.InsertAsync(accountEntity, token);

                    await TraceAccountEvent(accountEntity, "Register", info.Device, token: token);
                }

                if (info.Device != null)
                {
                    await LogDeviceInfo(info.Device, token);
                }

                return this.CreateSuccessResult(this.ToResponse<AccountRegisterResponse>(accountEntity));
            }
            catch (Exception ex)
            {
                //CustomTrace.TraceError(ex, "Account.Register Exception：AccoundName={info.AccountName}");
                throw;
            }
        }

        //---------------------------------------------------------------------
        [HttpPost]
        [Route("api/accounts/login")]
        public async Task<IActionResult> Login([FromBody]AccountLoginInfo info, CancellationToken token)
        {
            if (info == null || string.IsNullOrEmpty(info.AccountName))
            {
                throw new UCenterException(UCenterErrorCode.InvalidAccountName);
            }

            logger.LogInformation("Login AccName={0}", info.AccountName);

            var accountEntity = await this.Database.Accounts.GetSingleAsync(a => a.AccountName == info.AccountName, token);
            if (accountEntity == null)
            {
                await this.TraceAccountErrorAsync(
                     info.AccountName,
                     UCenterErrorCode.AccountNotExist,
                     "The account does not exist",
                     token);

                throw new UCenterException(UCenterErrorCode.AccountNotExist);
            }

            if (string.IsNullOrEmpty(info.Password)
                || !EncryptHelper.VerifyHash(info.Password, accountEntity.Password))
            {
                await this.TraceAccountErrorAsync(
                    accountEntity,
                    UCenterErrorCode.AccountPasswordUnauthorized,
                    "The account name and password do not match",
                    token);

                throw new UCenterException(UCenterErrorCode.AccountPasswordUnauthorized);
            }

            await AccountLoginAsync(accountEntity, token);

            if (info.Device != null && !string.IsNullOrEmpty(info.Device.Id))
            {
                await LogDeviceInfo(info.Device, token);
            }

            await this.TraceAccountEvent(accountEntity, "Login", info.Device, token: token);

            return this.CreateSuccessResult(this.ToResponse<AccountLoginResponse>(accountEntity));
        }

        //---------------------------------------------------------------------
        [HttpPost]
        [Route("api/accounts/wechatlogin")]
        public async Task<IActionResult> WeChatLogin([FromBody]AccountWeChatOAuthInfo info, CancellationToken token)
        {
            // info.AppId目前传值不正确，应等于settings.WechatAppId
            // 后续应当根据info.AppId去查找对应WechatAppSecret
            logger.LogInformation("WeChatLogin AppId=" + info.AppId);
            logger.LogInformation("WeChatLogin AppId=" + settings.WechatAppId);
            logger.LogInformation("WeChatLogin Code=" + info.Code);

            OAuthAccessTokenResult access_token_result = null;
            try
            {
                access_token_result = await OAuthApi.GetAccessTokenAsync(
                    settings.WechatAppId, settings.WechatAppSecret, info.Code);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());

                throw new UCenterException(UCenterErrorCode.AccountOAuthTokenUnauthorized);
            }

            if (access_token_result == null
                || access_token_result.errcode != 0)
            {
                logger.LogError("GetAccessTokenAsync失败");

                throw new UCenterException(UCenterErrorCode.AccountOAuthTokenUnauthorized);
            }

            OAuthUserInfo user_info = null;
            try
            {
                user_info = await OAuthApi.GetUserInfoAsync(
                access_token_result.access_token, access_token_result.openid);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());

                throw new UCenterException(UCenterErrorCode.AccountOAuthTokenUnauthorized);
            }

            if (user_info == null)
            {
                logger.LogError("OAuthUserInfo为空");

                throw new UCenterException(UCenterErrorCode.AccountOAuthTokenUnauthorized);
            }

            logger.LogInformation("OpenId=" + user_info.openid);
            logger.LogInformation("NickName=" + user_info.nickname);
            logger.LogInformation("Sex=" + user_info.sex);
            logger.LogInformation("Province=" + user_info.province);
            logger.LogInformation("City=" + user_info.city);
            logger.LogInformation("Country=" + user_info.country);
            logger.LogInformation("Headimgurl=" + user_info.headimgurl);
            logger.LogInformation("Unionid=" + user_info.unionid);
            if (user_info.privilege != null)
            {
                foreach (var i in user_info.privilege)
                {
                    if (i != null)
                    {
                        logger.LogInformation("Privilege=" + i);
                    }
                }
            }

            bool need_update_nickname = false;
            bool need_update_icon = false;

            // 查找AccountWechat
            var acc_wechat = await this.Database.AccountWechat.GetSingleAsync(
                a => a.Unionid == user_info.unionid
                && a.OpenId == user_info.openid
                && a.AppId == settings.WechatAppId,
                token);

            // 创建AccountWechat
            if (acc_wechat == null)
            {
                acc_wechat = new AccountWechatEntity()
                {
                    Id = Guid.NewGuid().ToString(),
                    AccountId = Guid.NewGuid().ToString(),
                    Unionid = user_info.unionid,
                    OpenId = user_info.openid,
                    AppId = settings.WechatAppId,
                    NickName = user_info.nickname,
                    Gender = (Gender)user_info.sex,
                    Province = user_info.province,
                    City = user_info.city,
                    Country = user_info.country,
                    Headimgurl = user_info.headimgurl
                };

                await this.Database.AccountWechat.InsertAsync(acc_wechat, token);

                need_update_nickname = true;
                need_update_icon = true;
            }
            else
            {
                if (acc_wechat.Headimgurl != user_info.headimgurl)
                {
                    acc_wechat.Headimgurl = user_info.headimgurl;
                    need_update_icon = true;
                }

                if (acc_wechat.NickName != user_info.nickname)
                {
                    acc_wechat.NickName = user_info.nickname;
                    need_update_nickname = true;
                }

                if (need_update_icon || need_update_nickname)
                {
                    await this.Database.AccountWechat.UpsertAsync(acc_wechat, token);
                }
            }

            // 查找Account
            var acc = await this.Database.Accounts.GetSingleAsync(
                acc_wechat.AccountId, token);

            // 创建Account
            if (acc == null)
            {
                acc = new AccountEntity()
                {
                    Id = acc_wechat.AccountId,
                    AccountName = Guid.NewGuid().ToString(),
                    AccountType = AccountType.NormalAccount,
                    AccountStatus = AccountStatus.Active,
                    //Password = EncryptHelper.ComputeHash(info.Password),
                    //SuperPassword = EncryptHelper.ComputeHash(info.SuperPassword),
                    Token = EncryptHashManager.GenerateToken(),
                    Gender = acc_wechat.Gender,
                    Identity = string.Empty,
                    Phone = string.Empty,
                    Email = string.Empty
                };

                await this.Database.Accounts.InsertAsync(acc, token);

                need_update_nickname = true;
                need_update_icon = true;
            }

            // 微信头像覆盖Acc头像
            if (need_update_icon &&
                !string.IsNullOrEmpty(user_info.headimgurl))
            {
                //logger.LogInformation("微信头像覆盖Acc头像，Headimgurl={0}", acc_wechat.Headimgurl);

                await DownloadWechatHeadIcon(user_info.headimgurl, acc.Id, token);

                acc.ProfileImage = user_info.headimgurl;
                await this.Database.Accounts.UpsertAsync(acc, token);
            }

            string current_nickname = string.Empty;
            var app = UCenterContext.Instance.CacheAppEntity.GetAppEntityByWechatAppId(settings.WechatAppId);
            var data_id = $"{app.Id}_{acc.Id}";
            var account_data = await this.Database.AppAccountDatas.GetSingleAsync(data_id, token);
            if (account_data != null)
            {
                var m = JsonConvert.DeserializeObject<Dictionary<string, string>>(account_data.Data);
                if (m.ContainsKey("nick_name"))
                {
                    current_nickname = m["nick_name"];
                }

                // 微信昵称覆盖Acc昵称
                if (current_nickname != acc_wechat.NickName && !string.IsNullOrEmpty(acc_wechat.NickName))
                {
                    m["nick_name"] = acc_wechat.NickName;
                    account_data.Data = JsonConvert.SerializeObject(m);

                    await this.Database.AppAccountDatas.UpsertAsync(account_data, token);
                }
            }
            else
            {
                Dictionary<string, string> m = new Dictionary<string, string>();
                m["nick_name"] = acc_wechat.NickName;

                account_data = new AppAccountDataEntity
                {
                    Id = data_id,
                    AppId = app.Id,
                    AccountId = acc.Id,
                    Data = JsonConvert.SerializeObject(m)
                };

                await this.Database.AppAccountDatas.UpsertAsync(account_data, token);
            }

            return this.CreateSuccessResult(this.ToResponse<AccountLoginResponse>(acc));
        }

        //---------------------------------------------------------------------
        [HttpPost]
        [Route("api/accounts/guestaccess")]
        public async Task<IActionResult> GuestAccess([FromBody]GuestAccessInfo info, CancellationToken token)
        {
            if (info == null)
            {
                throw new UCenterException(UCenterErrorCode.DeviceInfoNull);
            }

            if (string.IsNullOrEmpty(info.AppId))
            {
                throw new UCenterException(UCenterErrorCode.DeviceInfoNull);
            }

            EnsureDeviceInfo(info.Device);

            AccountEntity accountEntity;
            string guestDeviceId = $"{info.AppId}_{info.Device.Id}";
            var guestDeviceEntity = await this.Database.GuestDevices.GetSingleAsync(guestDeviceId, token);
            if (guestDeviceEntity == null)
            {
                accountEntity = new AccountEntity()
                {
                    Id = Guid.NewGuid().ToString(),
                    AccountName = Guid.NewGuid().ToString(),
                    AccountType = AccountType.Guest,
                    AccountStatus = AccountStatus.Active,
                    Token = EncryptHashManager.GenerateToken()
                };
                await this.Database.Accounts.InsertAsync(accountEntity, token);

                guestDeviceEntity = new GuestDeviceEntity()
                {
                    Id = $"{info.AppId}_{info.Device.Id}",
                    AccountId = accountEntity.Id,
                    AppId = info.AppId,
                    Device = info.Device
                };
                await this.Database.GuestDevices.InsertAsync(guestDeviceEntity, token);

                await this.TraceAccountEvent(accountEntity, "GuestRegister", info.Device, token: token);
            }
            else
            {
                accountEntity = await this.Database.Accounts.GetSingleAsync(guestDeviceEntity.AccountId, token);

                await this.TraceAccountEvent(accountEntity, "GuestLogin", info.Device, token: token);
            }

            await LogDeviceInfo(info.Device, token);

            var response = new GuestAccessResponse
            {
                AccountId = accountEntity.Id,
                AccountName = accountEntity.AccountName,
                AccountType = accountEntity.AccountType,
                Token = accountEntity.Token
            };

            return this.CreateSuccessResult(response);
        }

        //---------------------------------------------------------------------
        [HttpPost]
        [Route("api/accounts/resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody]AccountResetPasswordInfo info, CancellationToken token)
        {
            if (info == null
                || string.IsNullOrEmpty(info.AccountName))
            {
                throw new UCenterException(UCenterErrorCode.AccountNotExist);
            }

            var accountEntity = await this.Database.Accounts.GetSingleAsync(a => a.AccountName == info.AccountName, token);
            if (accountEntity == null)
            {
                throw new UCenterException(UCenterErrorCode.AccountNotExist);
            }

            if (!EncryptHelper.VerifyHash(info.SuperPassword, accountEntity.SuperPassword))
            {
                await this.TraceAccountErrorAsync(
                    accountEntity,
                    UCenterErrorCode.AccountPasswordUnauthorized,
                    "The super password provided is incorrect",
                    token);

                throw new UCenterException(UCenterErrorCode.AccountPasswordUnauthorized);
            }

            accountEntity.Password = EncryptHelper.ComputeHash(info.Password);
            await this.Database.Accounts.UpsertAsync(accountEntity, token);
            await this.TraceAccountEvent(accountEntity, "ResetPassword", token: token);

            return this.CreateSuccessResult(this.ToResponse<AccountResetPasswordResponse>(accountEntity));
        }

        //---------------------------------------------------------------------
        [HttpPost]
        [Route("api/accounts/{accountId}/upload")]
        public async Task<IActionResult> UploadProfileImage(string accountId, IFormFile file, CancellationToken token)
        {
            logger.LogInformation("UploadProfileImage AccId={0}", accountId);

            var account = await this.GetAndVerifyAccount(accountId, token);

            using (var stream = file.OpenReadStream())
            {
                var image = Image.FromStream(stream);

                using (var thumbnailStream = this.GetThumbprintStream(image))
                {
                    var smallProfileName = this.settings.ProfileThumbnailForBlobNameTemplate.FormatInvariant(accountId);

                    try
                    {
                        account.ProfileThumbnail =
                            await this.storageContext.UploadBlobAsync(smallProfileName, thumbnailStream, token);

                        logger.LogInformation("UploadProfileImage ProfileThumbnail=" + account.ProfileThumbnail);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.ToString());
                    }
                }

                try
                {
                    string profileName = this.settings.ProfileImageForBlobNameTemplate.FormatInvariant(accountId);
                    stream.Seek(0, SeekOrigin.Begin);
                    account.ProfileImage = await this.storageContext.UploadBlobAsync(profileName, stream, token);

                    logger.LogInformation("UploadProfileImage ProfileImage=" + account.ProfileImage);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.ToString());
                }

                await this.Database.Accounts.UpsertAsync(account, token);

                await this.TraceAccountEvent(account, "UploadProfileImage", token: token);

                logger.LogInformation("UploadProfileImage End");

                return this.CreateSuccessResult(this.ToResponse<AccountUploadProfileImageResponse>(account));
            }
        }

        //---------------------------------------------------------------------
        //[HttpGet]
        //[Route("api/accounts/ip")]
        //public IActionResult GetClientIp(CancellationToken token)
        //{
        //    string ipAddress = IPHelper.GetClientIpAddress(Request);
        //    return this.CreateSuccessResult(ipAddress);
        //}

        //---------------------------------------------------------------------
        private async Task DownloadWechatHeadIcon(string url, string account_id, CancellationToken token)
        {
            try
            {
                //Stream stream = await this.httpClient.GetStreamAsync(new Uri(url));
                var data = await webClient.DownloadDataTaskAsync(url);
                using (MemoryStream stream = new MemoryStream(data))
                {
                    var image = Image.FromStream(stream);

                    using (var thumbnailStream = this.GetThumbprintStream(image))
                    {
                        var smallProfileName = this.settings.ProfileThumbnailForBlobNameTemplate.FormatInvariant(account_id);

                        try
                        {
                            await this.storageContext.UploadBlobAsync(smallProfileName, thumbnailStream, token);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex.ToString());
                        }
                    }

                    try
                    {
                        string profileName = this.settings.ProfileImageForBlobNameTemplate.FormatInvariant(account_id);
                        stream.Seek(0, SeekOrigin.Begin);
                        await this.storageContext.UploadBlobAsync(profileName, stream, token);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
            }
        }

        //---------------------------------------------------------------------
        private async Task AccountLoginAsync(AccountEntity accountEntity, CancellationToken token)
        {
            if (accountEntity.AccountStatus == AccountStatus.Disabled)
            {
                await this.TraceAccountErrorAsync(
                     accountEntity,
                     UCenterErrorCode.AccountDisabled,
                     "The account is disabled",
                     token);

                throw new UCenterException(UCenterErrorCode.AccountDisabled);
            }

            accountEntity.Token = EncryptHashManager.GenerateToken();
            accountEntity.LastLoginDateTime = DateTime.UtcNow;

            var filter = Builders<AccountEntity>.Filter.Where(e => e.Id == accountEntity.Id);
            var update = Builders<AccountEntity>.Update
                .Set("Token", accountEntity.Token)
                .Set("LastLoginDateTime", accountEntity.LastLoginDateTime);
            await this.Database.Accounts.UpdateOneAsync<AccountEntity>(accountEntity, filter, update, token);
        }

        //---------------------------------------------------------------------
        private async Task TraceAccountErrorAsync(
            AccountEntity account,
            UCenterErrorCode code,
            string message = null,
            CancellationToken token = default(CancellationToken))
        {
            var clientIp = HttpContext.GetClientIpAddress();
            var accountErrorEvent = new AccountErrorEventEntity()
            {
                Id = Guid.NewGuid().ToString(),
                Code = code,
                ClientIp = clientIp,
                LoginArea = string.Empty,
                Message = message
            };

            if (account != null)
            {
                accountErrorEvent.AccountName = account.AccountName;
                accountErrorEvent.AccountId = account.Id;
            }

            await this.eventTrace.TraceEvent<AccountErrorEventEntity>(accountErrorEvent, token);
        }

        //---------------------------------------------------------------------
        private async Task TraceAccountErrorAsync(
            string AccountName,
            UCenterErrorCode code,
            string message = null,
            CancellationToken token = default(CancellationToken))
        {
            var clientIp = HttpContext.GetClientIpAddress();

            var accountErrorEvent = new AccountErrorEventEntity()
            {
                Id = Guid.NewGuid().ToString(),
                //AccountId = account.Id,
                AccountName = AccountName,
                Code = code,
                ClientIp = clientIp,
                LoginArea = string.Empty,
                Message = message
            };

            await this.eventTrace.TraceEvent<AccountErrorEventEntity>(accountErrorEvent, token);
        }

        //---------------------------------------------------------------------
        private async Task TraceAccountEvent(
            AccountEntity account,
            string eventName,
            DeviceInfo device = null,
            string message = null,
            CancellationToken token = default(CancellationToken))
        {
            var clientIp = HttpContext.GetClientIpAddress();

            var accountEventEntity = new AccountEventEntity
            {
                Id = Guid.NewGuid().ToString(),
                EventName = eventName,
                ClientIp = clientIp,
                LoginArea = string.Empty,
                UserAgent = string.Empty, // todo: Migrate to asp.net core Request.Headers.UserAgent.ToString(),
                Message = message
            };

            if (account != null)
            {
                accountEventEntity.AccountName = account.AccountName;
                accountEventEntity.AccountId = account.Id;
            }

            if (device != null)
            {
                accountEventEntity.DeviceId = device.Id;
            }

            await this.eventTrace.TraceEvent(accountEventEntity, token);
        }

        //---------------------------------------------------------------------
        private void EnsureDeviceInfo(DeviceInfo device)
        {
            if (device == null)
            {
                throw new UCenterException(UCenterErrorCode.DeviceInfoNull);
            }

            if (string.IsNullOrEmpty(device.Id))
            {
                throw new UCenterException(UCenterErrorCode.DeviceIdNull);
            }
        }

        //---------------------------------------------------------------------
        private async Task LogDeviceInfo(DeviceInfo device, CancellationToken token)
        {
            if (device == null) return;

            var deviceEntity = await this.Database.Devices.GetSingleAsync(d => d.Id == device.Id, token);
            if (deviceEntity == null)
            {
                deviceEntity = new DeviceEntity()
                {
                    Id = device.Id,
                    Name = device.Name,
                    Type = device.Type,
                    Model = device.Model,
                    OperationSystem = device.OperationSystem
                };
                await this.Database.Devices.InsertAsync(deviceEntity, token);
            }
            else
            {
                var filterDefinition = Builders<DeviceEntity>.Filter.Where(e => e.Id == deviceEntity.Id);
                var updateDefinition = Builders<DeviceEntity>.Update.Set(e => e.UpdatedTime, DateTime.UtcNow);
                await this.Database.Devices.UpdateOneAsync(deviceEntity, filterDefinition, updateDefinition, token);
            }
        }

        //---------------------------------------------------------------------
        private async Task<AccountEntity> GetAndVerifyAccount(string accountId, CancellationToken token)
        {
            var account = await this.Database.Accounts.GetSingleAsync(accountId, token);
            if (account == null)
            {
                await this.TraceAccountErrorAsync(
                     account,
                     UCenterErrorCode.AccountNotExist,
                     "The account does not exist",
                     token);

                throw new UCenterException(UCenterErrorCode.AccountNotExist);
            }

            return account;
        }

        //---------------------------------------------------------------------
        private TResponse ToResponse<TResponse>(AccountEntity entity)
            where TResponse : AccountRequestResponse
        {
            var res = new AccountResponse
            {
                AccountId = entity.Id,
                AccountName = entity.AccountName,
                Password = entity.Password,
                SuperPassword = entity.Password,
                Token = entity.Token,
                LastLoginDateTime = entity.LastLoginDateTime,
                Name = entity.Name,
                ProfileImage = entity.ProfileImage,
                ProfileThumbnail = entity.ProfileThumbnail,
                Gender = entity.Gender,
                Identity = entity.Identity,
                Phone = entity.Phone,
                Email = entity.Email
            };

            var response = Activator.CreateInstance<TResponse>();
            response.ApplyEntity(res);

            return response;
        }

        //---------------------------------------------------------------------
        private Stream GetThumbprintStream(Image sourceImage)
        {
            var stream = new MemoryStream();
            if (sourceImage.Width > this.settings.MaxThumbnailWidth
                || sourceImage.Height > this.settings.MaxThumbnailHeight)
            {
                var radio = Math.Min(
                    (double)this.settings.MaxThumbnailWidth / sourceImage.Width,
                    (double)this.settings.MaxThumbnailHeight / sourceImage.Height);

                var twidth = (int)(sourceImage.Width * radio);
                var theigth = (int)(sourceImage.Height * radio);
                var thumbnail = sourceImage.GetThumbnailImage(twidth, theigth, null, IntPtr.Zero);

                thumbnail.Save(stream, sourceImage.RawFormat);
            }
            else
            {
                sourceImage.Save(stream, sourceImage.RawFormat);
            }

            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        //---------------------------------------------------------------------
        private KeyPlaceholderEntity GenerateKeyPlaceholder(string name, KeyType type, string accountId, string accountName)
        {
            return new KeyPlaceholderEntity
            {
                Id = $"{type}-{name}",
                Name = name,
                Type = type,
                AccountId = accountId,
                AccountName = accountName
            };
        }

        //---------------------------------------------------------------------
        private void ValidateAccount(AccountRegisterRequestInfo account)
        {
            string accountNamePattern = @"^[a-zA-Z0-9.@]*$";
            var accountNameRegex = new Regex(accountNamePattern, RegexOptions.IgnoreCase);

            if (account == null)
            {
                throw new UCenterException(UCenterErrorCode.InvalidAccountName);
            }

            if (string.IsNullOrEmpty(account.AccountName)
                || !accountNameRegex.IsMatch(account.AccountName)
                || account.AccountName.Length < 4
                || account.AccountName.Length > 64)
            {
                throw new UCenterException(UCenterErrorCode.InvalidAccountName);
            }

            if (string.IsNullOrEmpty(account.Password)
                || account.Password.Length < 6
                || account.Password.Length > 64)
            {
                throw new UCenterException(UCenterErrorCode.InvalidAccountPassword);
            }

            if (string.IsNullOrEmpty(account.SuperPassword)
                || account.SuperPassword.Length < 6
                || account.Password.Length > 64)
            {
                throw new UCenterException(UCenterErrorCode.InvalidAccountPassword);
            }

            //if (string.IsNullOrEmpty(account.Phone))
            //{
            //    throw new UCenterException(UCenterErrorCode.InvalidAccountPhone);
            //}

            if (!string.IsNullOrEmpty(account.Email))
            {
                string emailPattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
                var emailRegex = new Regex(emailPattern, RegexOptions.IgnoreCase);

                if (!emailRegex.IsMatch(account.Email))
                {
                    throw new UCenterException(UCenterErrorCode.InvalidAccountEmail);
                }
            }
        }
    }
}