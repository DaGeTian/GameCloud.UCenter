using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace GameCloud.UCenter.Api.ApiControllers
{
    /// <summary>
    /// UCenter account API controller
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AccountApiController : ApiControllerBase
    {
        private readonly Settings settings;
        private readonly StorageAccountContext storageContext;
        private readonly EventTrace eventTrace;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountApiController" /> class.
        /// </summary>
        /// <param name="database">The database context.</param>
        /// <param name="settings">The UCenter settings.</param>
        /// <param name="storageContext">The storage account context.</param>
        /// <param name="eventTrace">The event trace instance.</param>
        [ImportingConstructor]
        public AccountApiController(
            UCenterDatabaseContext database,
            Settings settings,
            StorageAccountContext storageContext,
            EventTrace eventTrace)
            : base(database)
        {
            this.settings = settings;
            this.storageContext = storageContext;
            this.eventTrace = eventTrace;
        }

        /// <summary>
        /// Register account.
        /// </summary>
        /// <param name="info">Indicating the account information.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        [HttpPost]
        [Route("api/accounts/register")]
        public async Task<IActionResult> Register([FromBody]AccountRegisterRequestInfo info, CancellationToken token)
        {
            var removeTempsIfError = new List<KeyPlaceholderEntity>();
            var error = false;

            try
            {
                ValidateAccount(info);

                var accountEntity = new AccountEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    AccountName = info.AccountName,
                    AccountType = AccountType.NormalAccount,
                    AccountStatus = AccountStatus.Active,
                    Name = info.Name,
                    Email = info.Email,
                    Identity = info.Identity,
                    Password = EncryptHelper.ComputeHash(info.Password),
                    SuperPassword = EncryptHelper.ComputeHash(info.SuperPassword),
                    Phone = info.Phone,
                    Gender = info.Gender
                };

                var placeholders = new[]
                {
                    this.GenerateKeyPlaceholder(accountEntity.AccountName, KeyType.Name, accountEntity.Id, accountEntity.AccountName),
                    this.GenerateKeyPlaceholder(accountEntity.Phone, KeyType.Phone, accountEntity.Id, accountEntity.AccountName),
                    this.GenerateKeyPlaceholder(accountEntity.Email, KeyType.Email, accountEntity.Id, accountEntity.AccountName)
                };

                foreach (var placeholder in placeholders)
                {
                    if (!string.IsNullOrEmpty(placeholder.Name))
                    {
                        await this.Database.KeyPlaceholders.InsertAsync(placeholder, token);
                        removeTempsIfError.Add(placeholder);
                    }
                }

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

                if (info.Device != null)
                {
                    await LogDeviceInfo(info.Device, token);
                }

                await TraceAccountEvent(accountEntity, "Register", info.Device, token: token);

                return this.CreateSuccessResult(this.ToResponse<AccountRegisterResponse>(accountEntity));
            }
            catch (Exception ex)
            {
                CustomTrace.TraceError(ex, "Account.Register Exception：AccoundName={info.AccountName}");

                error = true;
                if (ex is MongoWriteException)
                {
                    var mex = ex as MongoWriteException;
                    if (mex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                    {
                        throw new UCenterException(UCenterErrorCode.AccountNameAlreadyExist, mex.Message);
                    }
                }

                throw;
            }
            finally
            {
                if (error)
                {
                    try
                    {
                        foreach (var item in removeTempsIfError)
                        {
                            this.Database.KeyPlaceholders.DeleteAsync(item, token).Wait(token);
                        }
                    }
                    catch (Exception ex)
                    {
                        CustomTrace.TraceError(ex, "Error to remove placeholder");
                    }
                }
            }
        }

        /// <summary>
        /// Login account.
        /// </summary>
        /// <param name="info">Indicating the account information.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        [HttpPost]
        [Route("api/accounts/login")]
        public async Task<IActionResult> Login([FromBody]AccountLoginInfo info, CancellationToken token)
        {
            var accountEntity = await this.Database.Accounts.GetSingleAsync(a => a.AccountName == info.AccountName, token);

            if (accountEntity == null)
            {
                await this.TraceUCenterErrorAsync(
                     accountEntity,
                     UCenterErrorCode.AccountNotExist,
                     "The account does not exist",
                     token);

                throw new UCenterException(UCenterErrorCode.AccountNotExist);
            }

            if (accountEntity.AccountStatus == AccountStatus.Disabled)
            {
                await this.TraceUCenterErrorAsync(
                     accountEntity,
                     UCenterErrorCode.AccountDisabled,
                     "The account is disabled",
                     token);

                throw new UCenterException(UCenterErrorCode.AccountDisabled);
            }

            if (!EncryptHelper.VerifyHash(info.Password, accountEntity.Password))
            {
                await this.TraceUCenterErrorAsync(
                    accountEntity,
                    UCenterErrorCode.AccountPasswordUnauthorized,
                    "The account name and password do not match",
                    token);

                throw new UCenterException(UCenterErrorCode.AccountPasswordUnauthorized);
            }

            accountEntity.Token = EncryptHashManager.GenerateToken();
            accountEntity.LastLoginDateTime = DateTime.UtcNow;

            var filter = Builders<AccountEntity>.Filter.Where(e => e.Id == accountEntity.Id);
            var update = Builders<AccountEntity>.Update
                .Set("Token", accountEntity.Token)
                .Set("LastLoginDateTime", accountEntity.LastLoginDateTime);
            await this.Database.Accounts.UpdateOneAsync<AccountEntity>(accountEntity, filter, update, token);

            if (info.Device != null)
            {
                await LogDeviceInfo(info.Device, token);
            }

            await this.TraceAccountEvent(accountEntity, "Login", info.Device, token: token);

            return this.CreateSuccessResult(this.ToResponse<AccountLoginResponse>(accountEntity));
        }

        /// <summary>
        /// Guest account login.
        /// </summary>
        /// <param name="info">Indicating the account information.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        [HttpPost]
        [Route("api/accounts/guestaccess")]
        public async Task<IActionResult> GuestAccess([FromBody]GuestAccessInfo info, CancellationToken token)
        {
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
            }
            else
            {
                accountEntity = await this.Database.Accounts.GetSingleAsync(guestDeviceEntity.AccountId, token);
            }

            await LogDeviceInfo(info.Device, token);

            await this.TraceAccountEvent(accountEntity, "GuestAccess", info.Device, token: token);

            var response = new GuestAccessResponse
            {
                AccountId = accountEntity.Id,
                AccountName = accountEntity.AccountName,
                AccountType = accountEntity.AccountType,
                Token = accountEntity.Token
            };

            return this.CreateSuccessResult(response);
        }

        /// <summary>
        /// Convert guest to account.
        /// </summary>
        /// <param name="info">Indicating the account information.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        [HttpPost]
        [Route("api/accounts/guestconvert")]
        public async Task<IActionResult> GuestConvert([FromBody]GuestConvertInfo info, CancellationToken token)
        {
            var accountEntity = await this.GetAndVerifyAccount(info.AccountId, token);

            accountEntity.AccountName = info.AccountName;
            accountEntity.AccountType = AccountType.NormalAccount;
            accountEntity.Name = info.Name;
            accountEntity.Identity = info.Identity;
            accountEntity.Password = EncryptHelper.ComputeHash(info.Password);
            accountEntity.SuperPassword = EncryptHelper.ComputeHash(info.SuperPassword);
            accountEntity.Phone = info.Phone;
            accountEntity.Email = info.Email;
            accountEntity.Gender = info.Gender;
            await this.Database.Accounts.UpsertAsync(accountEntity, token);

            try
            {
                await this.Database.GuestDevices.DeleteAsync(
                    d => d.AppId == info.AppId && d.AccountId == info.AccountId,
                    token);
            }
            catch (Exception ex)
            {
                CustomTrace.TraceError(ex, "Error to remove guest device");
            }

            await this.TraceAccountEvent(accountEntity, "GuestConvert", token: token);

            return this.CreateSuccessResult(this.ToResponse<AccountRegisterResponse>(accountEntity));
        }

        /// <summary>
        /// Reset account password.
        /// </summary>
        /// <param name="info">Indicating the account information.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        [HttpPost]
        [Route("api/accounts/resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody]AccountResetPasswordInfo info, CancellationToken token)
        {
            var accountEntity = await this.Database.Accounts.GetSingleAsync(a => a.AccountName == info.AccountName, token);
            if (accountEntity == null)
            {
                throw new UCenterException(UCenterErrorCode.AccountNotExist);
            }

            if (!EncryptHelper.VerifyHash(info.SuperPassword, accountEntity.SuperPassword))
            {
                await this.TraceUCenterErrorAsync(
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

        /// <summary>
        /// Upload account profile image.
        /// </summary>
        /// <param name="accountId">Indicating the account Id.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        [HttpPost]
        [Route("api/accounts/{accountId}/upload")]
        public async Task<IActionResult> UploadProfileImage(string accountId, IFormFile file, CancellationToken token)
        {
            var account = await this.GetAndVerifyAccount(accountId, token);

            using (var stream = file.OpenReadStream())
            {
                var image = Image.FromStream(stream);
                using (var thumbnailStream = this.GetThumbprintStream(image))
                {
                    var smallProfileName = this.settings.ProfileThumbnailForBlobNameTemplate.FormatInvariant(accountId);
                    account.ProfileThumbnail =
                        await this.storageContext.UploadBlobAsync(smallProfileName, thumbnailStream, token);
                }

                string profileName = this.settings.ProfileImageForBlobNameTemplate.FormatInvariant(accountId);
                stream.Seek(0, SeekOrigin.Begin);
                account.ProfileImage = await this.storageContext.UploadBlobAsync(profileName, stream, token);

                await this.Database.Accounts.UpsertAsync(account, token);
                await this.TraceAccountEvent(account, "UploadProfileImage", token: token);

                return this.CreateSuccessResult(this.ToResponse<AccountUploadProfileImageResponse>(account));
            }
        }

        /// <summary>
        /// Get client IP area.
        /// </summary>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        //[HttpGet]
        //[Route("api/accounts/ip")]
        //public IActionResult GetClientIp(CancellationToken token)
        //{
        //    string ipAddress = IPHelper.GetClientIpAddress(Request);
        //    return this.CreateSuccessResult(ipAddress);
        //}

        private async Task TraceUCenterErrorAsync(
            AccountEntity account,
            UCenterErrorCode code,
            string message = null,
            CancellationToken token = default(CancellationToken))
        {
            var clientIp = string.Empty;//IPHelper.GetClientIpAddress(Request);

            var errorEvent = new ErrorEventEntity()
            {
                Id = Guid.NewGuid().ToString(),
                AccountName = account.AccountName,
                AccountId = account.Id,
                Code = code,
                ClientIp = clientIp,
                LoginArea = string.Empty,
                Message = message
            };

            await this.Database.ErrorEvents.InsertAsync(errorEvent, token);
        }

        private async Task TraceAccountEvent(
            AccountEntity account,
            string eventName,
            DeviceInfo device = null,
            string message = null,
            CancellationToken token = default(CancellationToken))
        {
            var clientIp = string.Empty;// todo: migrate to asp.net core IPHelper.GetClientIpAddress(Request);

            var accountEventEntity = new AccountEventEntity
            {
                Id = Guid.NewGuid().ToString(),
                AccountName = account.AccountName,
                AccountId = account.Id,
                EventName = eventName,
                ClientIp = clientIp,
                LoginArea = string.Empty,
                UserAgent = string.Empty, // todo: Migrate to asp.net core Request.Headers.UserAgent.ToString(),
                Message = message
            };
            if (device != null)
            {
                accountEventEntity.DeviceId = device.Id;
            }

            await this.eventTrace.TraceEvent(accountEventEntity, token);
        }

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

        private async Task LogDeviceInfo(DeviceInfo device, CancellationToken token)
        {
            var deviceEntity = new DeviceEntity()
            {
                Id = device.Id,
                Name = device.Name,
                Type = device.Type,
                Model = device.Model,
                OperationSystem = device.OperationSystem
            };

            // todo: the event trace not handle device exists problem.
            await this.eventTrace.TraceEvent<DeviceEntity>(deviceEntity, token);
        }

        private async Task<AccountEntity> GetAndVerifyAccount(string accountId, CancellationToken token)
        {
            var account = await this.Database.Accounts.GetSingleAsync(accountId, token);
            if (account == null)
            {
                await this.TraceUCenterErrorAsync(
                     account,
                     UCenterErrorCode.AccountNotExist,
                     "The account does not exist",
                     token);

                throw new UCenterException(UCenterErrorCode.AccountNotExist);
            }

            return account;
        }

        private TResponse ToResponse<TResponse>(AccountEntity entity) where TResponse : AccountRequestResponse
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

        private void ValidateAccount(AccountRegisterRequestInfo account)
        {
            string accountNamePattern = @"^[a-zA-Z0-9.@]*$";
            var accountNameRegex = new Regex(accountNamePattern, RegexOptions.IgnoreCase);

            if (!accountNameRegex.IsMatch(account.AccountName))
            {
                throw new UCenterException(UCenterErrorCode.InvalidAccountName);
            }

            if (account.AccountName.Length < 4 || account.AccountName.Length > 64)
            {
                throw new UCenterException(UCenterErrorCode.InvalidAccountName);
            }

            if (account.Password.Length < 6 || account.Password.Length > 64)
            {
                throw new UCenterException(UCenterErrorCode.InvalidAccountPassword);
            }

            if (account.SuperPassword.Length < 6 || account.Password.Length > 64)
            {
                throw new UCenterException(UCenterErrorCode.InvalidAccountPassword);
            }

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