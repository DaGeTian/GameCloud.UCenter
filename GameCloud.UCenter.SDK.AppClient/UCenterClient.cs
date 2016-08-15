using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GameCloud.UCenter.Common.Portable.Models.AppClient;
using GameCloud.UCenter.Common.SDK;

namespace GameCloud.UCenter.SDK.AppClient
{
    /// <summary>
    /// Provide a UCenter client class.
    /// </summary>
    public class UCenterClient
    {
        private readonly string host;
        private readonly UCenterHttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="UCenterClient" /> class.
        /// </summary>
        /// <param name="host">Indicating the host address.</param>
        public UCenterClient(string host)
        {
            this.httpClient = new UCenterHttpClient();
            this.host = host.EndsWith("/") ? host.Substring(0, host.Length - 1) : host;
        }

        /// <summary>
        /// Get client ip address.
        /// </summary>
        /// <returns>Async response.</returns>
        public async Task<string> GetCleintIpAddressAsync()
        {
            var url = this.GenerateApiEndpoint("accounts", "ip");
            var result = await this.httpClient.SendAsyncWithException<string, string>(
                HttpMethod.Get,
                url,
                null);

            return result;
        }

        /// <summary>
        /// Register account.
        /// </summary>
        /// <param name="info">Indicating the account information.</param>
        /// <returns>Async response.</returns>
        public async Task<AccountRegisterResponse> AccountRegisterAsync(AccountRegisterInfo info)
        {
            var url = this.GenerateApiEndpoint("accounts", "register");
            var response = await this.httpClient.SendAsyncWithException<AccountRegisterInfo, AccountRegisterResponse>(
                HttpMethod.Post,
                url,
                info);

            return response;
        }

        /// <summary>
        /// Login account.
        /// </summary>
        /// <param name="info">Indicating the account information.</param>
        /// <returns>Async response.</returns>
        public async Task<AccountLoginResponse> AccountLoginAsync(AccountLoginInfo info)
        {
            var url = this.GenerateApiEndpoint("accounts", "login");
            var response = await this.httpClient.SendAsyncWithException<AccountLoginInfo, AccountLoginResponse>(
                HttpMethod.Post,
                url,
                info);

            return response;
        }

        /// <summary>
        /// Login guest account.
        /// </summary>
        /// <param name="device">Indicating the account device information.</param>
        /// <returns>Async response.</returns>
        public async Task<GuestAccessResponse> GuestAccessAsync(GuestAccessInfo info)
        {
            var url = this.GenerateApiEndpoint("accounts", "guestaccess");
            var response = await this.httpClient.SendAsyncWithException<GuestAccessInfo, GuestAccessResponse>(
                HttpMethod.Post,
                url,
                info);

            return response;
        }

        /// <summary>
        /// Convert account.
        /// </summary>
        /// <param name="info">Indicating the account information.</param>
        /// <returns>Async response.</returns>
        public async Task<GuestConvertResponse> GuestConvertAsync(GuestConvertInfo info)
        {
            var url = this.GenerateApiEndpoint("accounts", "guestconvert");
            var response = await this.httpClient.SendAsyncWithException<GuestConvertInfo, GuestConvertResponse>(
                HttpMethod.Post,
                url,
                info);

            return response;
        }

        /// <summary>
        /// Reset account password.
        /// </summary>
        /// <param name="info">Indicating the account information.</param>
        /// <returns>Async response.</returns>
        public async Task<AccountResetPasswordResponse> AccountResetPasswordAsync(AccountResetPasswordInfo info)
        {
            var url = this.GenerateApiEndpoint("accounts", "resetpassword");
            return await this.httpClient.SendAsyncWithException<AccountResetPasswordInfo, AccountResetPasswordResponse>(
                HttpMethod.Post,
                url,
                info);
        }

        /// <summary>
        /// Upload account profile image.
        /// </summary>
        /// <param name="accountId">Indicating the account id.</param>
        /// <param name="imagePath">Indicating the image file path.</param>
        /// <returns>Async response.</returns>
        public async Task<AccountUploadProfileImageResponse> AccountUploadProfileImagesync(
            string accountId,
            string imagePath)
        {
            const int BufferSize = 1024 * 1024;
            using (var stream = new FileStream(
                imagePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                BufferSize,
                true))
            {
                return await this.AccountUploadProfileImagesync(accountId, stream);
            }
        }

        /// <summary>
        /// Register account.
        /// </summary>
        /// <param name="accountId">Indicating the account id.</param>
        /// <param name="imageStream">Indicating the image stream.</param>
        /// <returns>Async response.</returns>
        public async Task<AccountUploadProfileImageResponse> AccountUploadProfileImagesync(
            string accountId,
            Stream imageStream)
        {
            var url = this.GenerateApiEndpoint("accounts", $"{accountId}/upload");
            var content = new StreamContent(imageStream);
            content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            return await this.httpClient.SendAsyncWithException<HttpContent, AccountUploadProfileImageResponse>(
                HttpMethod.Post,
                url,
                content);
        }

        /// <summary>
        /// Get application configuration.
        /// </summary>
        /// <param name="appId">Indicating the application id.</param>
        /// <returns>Async response.</returns>
        public async Task<AppConfigurationResponse> GetAppConfigurationAsync(string appId)
        {
            var url = this.GenerateApiEndpoint("apps", $"{appId}/configurations");
            var response = await this.httpClient.SendAsyncWithException<string, AppConfigurationResponse>(
                HttpMethod.Get,
                url,
                null);

            return response;
        }

        private string GenerateApiEndpoint(string controller, string route, string queryString = null)
        {
            var url = $"{this.host}/api/{controller}/{route}";
            if (!string.IsNullOrEmpty(queryString))
            {
                url = $"{url}/?{queryString}";
            }

            return url;
        }
    }
}