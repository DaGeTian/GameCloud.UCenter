namespace GF.UCenter.Web
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Common.Logger;
    using CouchBase;
    using Newtonsoft.Json.Linq;
    using UCenter.Common;
    using UCenter.Common.Portable;
    using UCenter.Common.Settings;

    /// <summary>
    ///     UCenter payment api controller
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/payment")]
    public class PaymentApiController : ApiControllerBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PaymentApiController" /> class.
        /// </summary>
        /// <param name="db">The couch base context.</param>
        /// <param name="settings">The UCenter settings.</param>
        [ImportingConstructor]
        public PaymentApiController(CouchBaseContext db, Settings settings)
            : base(db)
        {
        }

        [Route("charge")]
        public async Task<IHttpActionResult> CreateCharge([FromBody] ChargeInfo info)
        {
            CustomTrace.TraceInformation($"AppServer.CreateCharge\nAppId={info.AppId}\nAccountId={info.AccountId}");

            try
            {
                var orderEntity = new OrderEntity
                {
                    AppId = info.AppId,
                    AccountId = info.AccountId,
                    OrderStatus = OrderStatus.Created,
                    CreatedTime = DateTime.UtcNow
                };

                await this.DatabaseContext.Bucket.InsertSlimAsync(orderEntity);

                // TODO: Replace with live key
                Pingpp.Pingpp.SetApiKey("sk_test_zXnD8KKOyfn1vDuj9SG8ibfT");
                // TODO: Fix hard code path
                Pingpp.Pingpp.SetPrivateKeyPath(@"C:\github\GF.UCenter\GF.UCenter.Web\App_Data\rsa_private_key.pem");

                var appId = "app_H4yDu5COi1O4SWvz";
                var r = new Random();
                string orderNoPostfix = r.Next(0, 1000000).ToString("D6");
                string orderNo = $"{DateTime.Now.ToString("yyyyMMddHHmmssffff")}_{orderNoPostfix}";
                var amount = info.Amount;
                var channel = "alipay";
                var currency = "cny";

                //交易请求参数，这里只列出必填参数，可选参数请参考 https://pingxx.com/document/api#api-c-new
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

                return CreateSuccessResult(charge);
            }
            catch (Exception ex)
            {
                CustomTrace.TraceError(ex, "Failed to create charge");
                throw new UCenterException(UCenterErrorCode.PaymentCreateChargeFailed, ex.Message);
            }
        }

        [HttpPost]
        [Route("webhook")]
        public async Task<IHttpActionResult> PingPlusPlusWebHook()
        {
            CustomTrace.TraceInformation("AppServer.PingPlusPlusWebHook");

            //获取 post 的 event对象
            var inputData = Request.Content.ReadAsStringAsync().Result;
            CustomTrace.TraceInformation("Received event\n" + inputData);

            //获取 header 中的签名
            IEnumerable<string> headerValues;
            string sig = string.Empty;
            if (Request.Headers.TryGetValues("x-pingplusplus-signature", out headerValues))
            {
                sig = headerValues.FirstOrDefault();
            }

            //公钥路径（请检查你的公钥 .pem 文件存放路径）
            string path = @"C:\github\GF.UCenter\GF.UCenter.Web\App_Data\rsa_public_key.pem";

            //验证签名
            VerifySignedHash(inputData, sig, path);

            await ProcessOrderAsync(inputData);

            return Ok();
        }

        public async Task ProcessOrderAsync(string orderData)
        {
            var jObject = JObject.Parse(orderData);
            var eventType = jObject.SelectToken("type");
            var orderNo = jObject.SelectToken("data.object.order_no");

            var order = await DatabaseContext.Bucket.GetByEntityIdSlimAsync<OrderEntity>(orderNo.ToString(), false);
            if (order == null)
            {
                order = new OrderEntity
                {
                    Id = orderNo.ToString(),
                    CreatedTime = DateTime.UtcNow
                };
            }

            order.OrderStatus = eventType.ToString() == "charge.succeeded" || eventType.ToString() == "refund.succeeded"
                ? OrderStatus.Success
                : OrderStatus.Failed;
            order.RawData = orderData;
            order.CompletedTime = DateTime.UtcNow;

            await DatabaseContext.Bucket.UpsertSlimAsync(order);
        }

        public static string VerifySignedHash(string str_DataToVerify, string str_SignedData,
            string str_publicKeyFilePath)
        {
            byte[] SignedData = Convert.FromBase64String(str_SignedData);

            var ByteConverter = new UTF8Encoding();
            byte[] DataToVerify = ByteConverter.GetBytes(str_DataToVerify);
            try
            {
                string sPublicKeyPEM = File.ReadAllText(str_publicKeyFilePath);
                var rsa = new RSACryptoServiceProvider();

                rsa.PersistKeyInCsp = false;
                rsa.LoadPublicKeyPEM(sPublicKeyPEM);

                if (rsa.VerifyData(DataToVerify, "SHA256", SignedData))
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
    }
}