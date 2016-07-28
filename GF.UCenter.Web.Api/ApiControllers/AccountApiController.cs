namespace GF.UCenter.Web.Api.ApiControllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Text.RegularExpressions;
    using System.Web.Http;
    using Common;
    using Common.Logger;
    using MongoDB;
    using MongoDB.Adapters;
    using global::MongoDB.Driver;
    using MongoDB.Entity;
    using UCenter.Common;
    using UCenter.Common.IP;
    using UCenter.Common.Portable.Contracts;
    using UCenter.Common.Portable.Exceptions;
    using UCenter.Common.Portable.Models.AppClient;
    using UCenter.Common.Portable.Models.Ip;
    using UCenter.Common.Settings;

    /// <summary>
    /// UCenter account API controller
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/account")]
    public class AccountApiController : ApiControllerBase
    {
        private readonly Settings settings;
        private readonly StorageAccountContext storageContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountApiController" /> class.
        /// </summary>
        /// <param name="database">The database context.</param>
        /// <param name="settings">The UCenter settings.</param>
        /// <param name="storageContext">The storage account context.</param>
        [ImportingConstructor]
        public AccountApiController(DatabaseContext database, Settings settings, StorageAccountContext storageContext)
            : base(database)
        {
            this.settings = settings;
            this.storageContext = storageContext;
        }

        /// <summary>
        /// Register account.
        /// </summary>
        /// <param name="info">Indicating the account information.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        [HttpPost]
        [Route("register")]
        public async Task<IHttpActionResult> Register([FromBody] AccountRegisterRequestInfo info, CancellationToken token)
        {
            CustomTrace.TraceInformation($"Account.Register AccountName={info.AccountName}");

            if (!ValidateAccountName(info.AccountName))
            {
                // TODO: Change to AccountRegisterFailedInvalidName in next client refresh
                throw new UCenterException(UCenterErrorCode.AccountRegisterFailedAlreadyExist);
            }

            var removeTempsIfError = new List<KeyPlaceholderEntity>();
            var error = false;
            try
            {
                var account = await this.Database.Accounts.GetSingleAsync(a => a.AccountName == info.AccountName, token);

                if (account != null)
                {
                    throw new UCenterException(UCenterErrorCode.AccountRegisterFailedAlreadyExist);
                }

                account = new AccountEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    AccountName = info.AccountName,
                    IsGuest = false,
                    Name = info.Name,
                    Email = info.Email,
                    IdentityNum = info.IdentityNum,
                    Password = EncryptHashManager.ComputeHash(info.Password),
                    SuperPassword = EncryptHashManager.ComputeHash(info.SuperPassword),
                    PhoneNum = info.PhoneNum,
                    Gender = info.Gender
                };

                var placeholders = new[]
                {
                    this.GenerateKeyPlaceholder(account.AccountName, KeyType.Name, account.Id, account.AccountName),
                    this.GenerateKeyPlaceholder(account.PhoneNum, KeyType.Phone, account.Id, account.AccountName),
                    this.GenerateKeyPlaceholder(account.Email, KeyType.Email, account.Id, account.AccountName)
                };

                foreach (var placeholder in placeholders)
                {
                    if (!string.IsNullOrEmpty(placeholder.Name))
                    {
                        await this.Database.KeyPlaceholders.InsertAsync(placeholder, token);
                        removeTempsIfError.Add(placeholder);
                    }
                }

                // set the default profiles
                account.ProfileImage = await this.storageContext.CopyBlobAsync(
                    account.Gender == Gender.Female ? this.settings.DefaultProfileImageForFemaleBlobName : this.settings.DefaultProfileImageForMaleBlobName,
                    this.settings.ProfileImageForBlobNameTemplate.FormatInvariant(account.Id),
                    token);

                account.ProfileThumbnail = await this.storageContext.CopyBlobAsync(
                    account.Gender == Gender.Female
                        ? this.settings.DefaultProfileThumbnailForFemaleBlobName
                        : this.settings.DefaultProfileThumbnailForMaleBlobName,
                    this.settings.ProfileThumbnailForBlobNameTemplate.FormatInvariant(account.Id),
                    token);

                await this.Database.Accounts.InsertAsync(account, token);

                return this.CreateSuccessResult(this.ToResponse<AccountRegisterResponse>(account));
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
                        throw new UCenterException(UCenterErrorCode.AccountRegisterFailedAlreadyExist, ex);
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
        [Route("login")]
        public async Task<IHttpActionResult> Login([FromBody] AccountLoginInfo info, CancellationToken token)
        {
            CustomTrace.TraceInformation($"Account.Login AccountName={info.AccountName}");

            var account = await this.Database.Accounts.GetSingleAsync(a => a.AccountName == info.AccountName, token);

            if (account == null)
            {
                throw new UCenterException(UCenterErrorCode.AccountNotExist);
            }

            if (!EncryptHashManager.VerifyHash(info.Password, account.Password))
            {
                await this.RecordLogin(
                    account,
                    UCenterErrorCode.AccountLoginFailedPasswordNotMatch,
                    "The account name and password do not match",
                    token);

                throw new UCenterException(UCenterErrorCode.AccountLoginFailedPasswordNotMatch);
            }

            account.LastLoginDateTime = DateTime.UtcNow;
            account.Token = EncryptHashManager.GenerateToken();
            await this.Database.Accounts.UpsertAsync(account, token);
            await this.RecordLogin(account, UCenterErrorCode.Success, token: token);

            return this.CreateSuccessResult(this.ToResponse<AccountLoginResponse>(account));
        }

        /// <summary>
        /// Guest account login.
        /// </summary>
        /// <param name="info">Indicating the account information.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        [HttpPost]
        [Route("guest")]
        public async Task<IHttpActionResult> GuestLogin([FromBody] AccountLoginInfo info, CancellationToken token)
        {
            CustomTrace.TraceInformation("Account.GuestLogin");

            var r = new Random();
            string accountNamePostfix = r.Next(0, 1000000).ToString("D6");
            string accountName = $"uc_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}_{accountNamePostfix}";
            string accountToken = EncryptHashManager.GenerateToken();
            string password = Guid.NewGuid().ToString();

            var account = new AccountEntity
            {
                Id = Guid.NewGuid().ToString(),
                AccountName = accountName,
                IsGuest = true,
                Password = EncryptHashManager.ComputeHash(password),
                Token = EncryptHashManager.GenerateToken()
            };

            await this.Database.Accounts.InsertAsync(account, token);

            var response = new AccountGuestLoginResponse
            {
                AccountId = account.Id,
                AccountName = accountName,
                Token = accountToken,
                Password = password
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
        [Route("convert")]
        public async Task<IHttpActionResult> Convert([FromBody] AccountConvertInfo info, CancellationToken token)
        {
            CustomTrace.TraceInformation($"Account.Convert AccountName={info.AccountName}");

            var account = await this.GetAndVerifyAccount(info.AccountId, token);

            if (!EncryptHashManager.VerifyHash(info.OldPassword, account.Password))
            {
                await this.RecordLogin(
                    account,
                    UCenterErrorCode.AccountLoginFailedPasswordNotMatch,
                    "The account name and password do not match",
                    token);

                throw new UCenterException(UCenterErrorCode.AccountLoginFailedPasswordNotMatch);
            }

            account.AccountName = info.AccountName;
            account.IsGuest = false;
            account.Name = info.Name;
            account.IdentityNum = info.IdentityNum;
            account.Password = EncryptHashManager.ComputeHash(info.Password);
            account.SuperPassword = EncryptHashManager.ComputeHash(info.SuperPassword);
            account.PhoneNum = info.PhoneNum;
            account.Email = info.Email;
            account.Gender = info.Gender;
            await this.Database.Accounts.UpsertAsync(account, token);

            await this.RecordLogin(account, UCenterErrorCode.Success, "Account converted successfully.", token);
            return this.CreateSuccessResult(this.ToResponse<AccountRegisterResponse>(account));
        }

        /// <summary>
        /// Reset account password.
        /// </summary>
        /// <param name="info">Indicating the account information.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        [HttpPost]
        [Route("resetpassword")]
        public async Task<IHttpActionResult> ResetPassword([FromBody] AccountResetPasswordInfo info, CancellationToken token)
        {
            CustomTrace.TraceInformation($"Account.ResetPassword AccountName={info.AccountName}");

            var account = await this.Database.Accounts.GetSingleAsync(a => a.AccountName == info.AccountName, token);
            if (account == null)
            {
                throw new UCenterException(UCenterErrorCode.AccountNotExist);
            }

            if (!EncryptHashManager.VerifyHash(info.SuperPassword, account.SuperPassword))
            {
                await this.RecordLogin(
                    account,
                    UCenterErrorCode.AccountLoginFailedPasswordNotMatch,
                    "The super password provided is incorrect",
                    token);

                throw new UCenterException(UCenterErrorCode.AccountLoginFailedPasswordNotMatch);
            }

            account.Password = EncryptHashManager.ComputeHash(info.Password);
            await this.Database.Accounts.UpsertAsync(account, token);
            await this.RecordLogin(account, UCenterErrorCode.Success, "Reset password successfully.", token);
            return this.CreateSuccessResult(this.ToResponse<AccountResetPasswordResponse>(account));
        }

        /// <summary>
        /// Upload account profile image.
        /// </summary>
        /// <param name="accountId">Indicating the account Id.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        [HttpPost]
        [Route("upload/{accountId}")]
        public async Task<IHttpActionResult> UploadProfileImage([FromUri] string accountId, CancellationToken token)
        {
            CustomTrace.TraceInformation($"Account.UploadProfileImage AccountId={accountId}");

            var account = await this.GetAndVerifyAccount(accountId, token);

            using (var stream = await this.Request.Content.ReadAsStreamAsync())
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
                await this.RecordLogin(account, UCenterErrorCode.Success, "Profile image uploaded successfully.", token);
                return this.CreateSuccessResult(
                    this.ToResponse<AccountUploadProfileImageResponse>(account));
            }
        }

        private async Task RecordLogin(
            AccountEntity account,
            UCenterErrorCode code,
            string comments = null,
            CancellationToken token = default(CancellationToken))
        {
            var clientIp = IPHelper.GetClientIpAddress(Request);
            var ipInfoResponse = await IPHelper.GetIPInfoAsync(clientIp, CancellationToken.None);
            string area;
            if (ipInfoResponse != null && ipInfoResponse.Code == IPInfoResponseCode.Success)
            {
                area = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}-{1}",
                    ipInfoResponse.Content.Country,
                    ipInfoResponse.Content.City ?? ipInfoResponse.Content.County);
            }
            else
            {
                area = string.Empty;
            }

            var record = new LoginRecordEntity
            {
                Id = Guid.NewGuid().ToString(),
                AccountName = account.AccountName,
                AccountId = account.Id,
                Code = code,
                LoginTime = DateTime.UtcNow,
                UserAgent = Request.Headers.UserAgent.ToString(),
                ClientIp = clientIp,
                LoginArea = area,
                Comments = comments
            };

            await this.Database.LoginRecords.InsertAsync(record, token);
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
                IdentityNum = entity.IdentityNum,
                PhoneNum = entity.PhoneNum,
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

        private bool ValidateAccountName(string accountName)
        {
            string pattern = @"^[a-zA-Z0-9.@]*$";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);

            return regex.IsMatch(accountName);
        }
    }
}