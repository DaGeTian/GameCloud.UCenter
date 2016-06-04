namespace GF.UCenter.Web
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Hosting;
    using System.Web.Http;
    using Common.Logger;
    using MongoDB;
    using MongoDB.Adapters;
    using MongoDB.Entity;
    using Newtonsoft.Json.Linq;
    using UCenter.Common;
    using UCenter.Common.Portable;
    using UCenter.Common.Settings;

    /// <summary>
    /// UCenter payment api controller
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/payment")]
    public class PaymentApiController : ApiControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentApiController" /> class.
        /// </summary>
        /// <param name="database">The database context.</param>
        [ImportingConstructor]
        public PaymentApiController(DatabaseContext database)
            : base(database)
        {
        }

        [Route("charge")]
        public async Task<IHttpActionResult> CreateCharge([FromBody] ChargeInfo info, CancellationToken token)
        {
            CustomTrace.TraceInformation($"AppServer.CreateCharge\nAppId={info.AppId}\nAccountId={info.AccountId}");

            try
            {
                var account = await this.Database.Accounts.GetSingleAsync(info.AccountId, token);
                var app = await this.Database.Apps.GetSingleAsync(info.AppId, token);
                var orderEntity = new OrderEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    AppId = info.AppId,
                    AppName = app == null ? null : app.Name,
                    AccountId = info.AccountId,
                    AccountName = account == null ? null : account.AccountName,
                    State = OrderState.Created,
                    CreatedTime = DateTime.UtcNow
                };

                await this.Database.Orders.InsertAsync(orderEntity, token);

                // TODO: Replace with live key
                Pingpp.Pingpp.SetApiKey("sk_test_zXnD8KKOyfn1vDuj9SG8ibfT");

                // TODO: Fix hard code path
                var certFile = this.GetCertFilePath("rsa_private_key.pem");
                Pingpp.Pingpp.SetPrivateKeyPath(certFile);

                var appId = "app_H4yDu5COi1O4SWvz";
                var r = new Random();
                string orderNoPostfix = r.Next(0, 1000000).ToString("D6");
                string orderNo = $"{DateTime.Now.ToString("yyyyMMddHHmmssffff")}_{orderNoPostfix}";
                var amount = info.Amount;
                var channel = "alipay";
                var currency = "cny";

                // 交易请求参数，这里只列出必填参数，可选参数请参考 https://pingxx.com/document/api#api-c-new
                var chParams = new Dictionary<string, object>
                {
                    {"order_no", new Random().Next(1, 999999999)},
                    {"amount", amount},
                    {"channel", "wx"},
                    {"currency", "cny"},
                    {"subject", info.Subject},
                    {"body", info.Body},
                    {"client_ip", "127.0.0.1"},
                    {"app", new Dictionary<string, string> {{"id", appId}}}
                };

                var charge = Pingpp.Models.Charge.Create(chParams);

                return this.CreateSuccessResult(charge);
            }
            catch (Exception ex)
            {
                CustomTrace.TraceError(ex, "Failed to create charge");
                throw new UCenterException(UCenterErrorCode.PaymentCreateChargeFailed, ex.Message);
            }
        }

        [HttpPost]
        [Route("webhook")]
        public async Task<IHttpActionResult> PingPlusPlusWebHook(CancellationToken token)
        {
            CustomTrace.TraceInformation("AppServer.PingPlusPlusWebHook");

            // 获取 post 的 event对象
            var inputData = Request.Content.ReadAsStringAsync().Result;
            CustomTrace.TraceInformation("Received event\n" + inputData);

            // 获取 header 中的签名
            IEnumerable<string> headerValues;
            string sig = string.Empty;
            if (Request.Headers.TryGetValues("x-pingplusplus-signature", out headerValues))
            {
                sig = headerValues.FirstOrDefault();
            }

            // 公钥路径（请检查你的公钥 .pem 文件存放路径）
            string certPath = this.GetCertFilePath("rsa_public_key.pem");

            // 验证签名
            VerifySignedHash(inputData, sig, certPath);

            await this.ProcessOrderAsync(inputData, token);

            return this.Ok();
        }

        internal async Task ProcessOrderAsync(string orderData, CancellationToken token)
        {
            var jObject = JObject.Parse(orderData);
            var eventType = jObject.SelectToken("type");
            var orderNo = jObject.SelectToken("data.object.order_no");

            var order = await this.Database.Orders.GetSingleAsync(orderNo.ToString(), token);

            if (order == null)
            {
                // todo: how to set the account id?
                order = new OrderEntity
                {
                    Id = orderNo.ToString(),
                    CreatedTime = DateTime.UtcNow
                };
            }

            order.State = eventType.ToString() == "charge.succeeded" || eventType.ToString() == "refund.succeeded"
                ? OrderState.Success
                : OrderState.Failed;

            order.Content = orderData;
            order.CompletedTime = DateTime.UtcNow;

            await this.Database.Orders.UpsertAsync(order, token);
        }

        private static string VerifySignedHash(string str_DataToVerify, string str_SignedData,
            string str_publicKeyFilePath)
        {
            byte[] signedData = Convert.FromBase64String(str_SignedData);

            var byteConverter = new UTF8Encoding();
            byte[] dataToVerify = byteConverter.GetBytes(str_DataToVerify);
            try
            {
                string sPublicKeyPEM = File.ReadAllText(str_publicKeyFilePath);
                var rsa = new RSACryptoServiceProvider();

                rsa.PersistKeyInCsp = false;
                rsa.LoadPublicKeyPEM(sPublicKeyPEM);

                if (rsa.VerifyData(dataToVerify, "SHA256", signedData))
                {
                    return "verify success";
                }
                return "verify fail";
            }
            catch (CryptographicException e)
            {
                CustomTrace.TraceError(e);
                return "verify error";
            }
        }

        private string GetCertFilePath(string certName)
        {
            // following not worked.
            // var path = HostingEnvironment.MapPath($"~/App_Data/{certName}");
            // todo: not sure if the followring logic can work well on iis.
            var path = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", certName);
            return path;
        }
    }
}