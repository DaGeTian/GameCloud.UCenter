namespace GF.UCenter.Web
{
    using System.ComponentModel.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Common;
    using Common.Logger;
    using MongoDB;
    using MongoDB.Adapters;
    using UCenter.Common;
    using UCenter.Common.IP;
    using UCenter.Common.Portable;
    using UCenter.Common.Settings;

    /// <summary>
    /// UCenter account api controller
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/appclient")]
    public class AppClientApiController : ApiControllerBase
    {
        private readonly StorageAccountContext storageContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountApiController" /> class.
        /// </summary>
        /// <param name="database">The database context.</param>
        /// <param name="settings">The UCenter settings.</param>
        /// <param name="storageContext">The storage account context.</param>
        [ImportingConstructor]
        public AppClientApiController(DatabaseContext database, Settings settings, StorageAccountContext storageContext)
            : base(database)
        {
            this.storageContext = storageContext;
        }

        [HttpPost]
        [Route("ip")]
        public async Task<IHttpActionResult> GetClientIpArea(CancellationToken token)
        {
            CustomTrace.TraceInformation("AppClient.GetClientIpArea");

            string ipAddress = IPHelper.GetClientIpAddress(Request);
            var response = await IPHelper.GetIPInfoAsync(ipAddress, token);
            return this.CreateSuccessResult(response);
        }

        [HttpPost]
        [Route("conf")]
        public async Task<IHttpActionResult> GetAppConfiguration([FromUri]string appId, CancellationToken token)
        {
            CustomTrace.TraceInformation($"AppClient.GetAppConfiguration AppId={appId}");

            var app = await this.Database.Apps.GetSingleAsync(appId, token);

            if (app == null)
            {
                throw new UCenterException(UCenterErrorCode.AppNotExit);
            }
            var response = new AppConfigurationResponse()
            {
                AppId = app.Id,
                Configuration = app.Configuration
            };
            return this.CreateSuccessResult(response);
        }
    }
}