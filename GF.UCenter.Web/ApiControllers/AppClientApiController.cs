namespace GF.UCenter.Web
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Common;
    using Common.IP;
    using Common.Portable;
    using Common.Settings;
    using CouchBase;

    /// <summary>
    ///     UCenter account api controller
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/appclient")]
    public class AppClientApiController : ApiControllerBase
    {
        private readonly Settings settings;
        private readonly StorageAccountContext storageContext;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AccountApiController" /> class.
        /// </summary>
        /// <param name="db">The couch base context.</param>
        /// <param name="settings">The UCenter settings.</param>
        /// <param name="storageContext">The storage account context.</param>
        [ImportingConstructor]
        public AppClientApiController(CouchBaseContext db, Settings settings, StorageAccountContext storageContext)
            : base(db)
        {
            this.settings = settings;
            this.storageContext = storageContext;
        }

        [HttpPost]
        [Route("ip")]
        public async Task<IHttpActionResult> GetClientIpArea()
        {
            Logger.Info("AppClient.GetClientIpArea");

            string ipAddress = IPHelper.GetClientIpAddress(Request);
            var response = await IPHelper.GetIPInfoAsync(ipAddress, CancellationToken.None);
            return CreateSuccessResult(response);
        }

        [HttpPost]
        [Route("conf")]
        public async Task<IHttpActionResult> GetAppConfiguration([FromUri]string appId)
        {
            Logger.Info($"AppClient.GetAppConfiguration AppId={appId}");

            var app = await this.DatabaseContext.Bucket.GetByEntityIdSlimAsync<AppEntity>(appId, false);
            if (app == null)
            {
                throw new UCenterException(UCenterErrorCode.AppNotExit);
            }
            var response = new AppConfigurationResponse()
            {
                AppId = app.Id,
                Configuration = app.Configuration
            };
            return CreateSuccessResult(response);
        }
    }
}