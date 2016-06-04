using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using GF.UCenter.Common;
using GF.UCenter.Common.IP;
using GF.UCenter.Common.Portable;
using GF.UCenter.Common.Settings;
using GF.UCenter.MongoDB;
using GF.UCenter.MongoDB.Adapters;
using GF.UCenter.Web.Common;
using GF.UCenter.Web.Common.Logger;

namespace GF.UCenter.Web.Api.ApiControllers
{
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
        /// <param name="settings">The database context.</param>
        /// <param name="storageContext">The UCenter settings.</param>
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