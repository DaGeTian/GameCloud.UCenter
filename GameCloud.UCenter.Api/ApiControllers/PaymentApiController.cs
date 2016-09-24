using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameCloud.Database.Adapters;
using GameCloud.UCenter.Api.Extensions;
using GameCloud.UCenter.Common.Settings;
using GameCloud.UCenter.Common.Models.PingPlusPlus;
using GameCloud.UCenter.Database;
using GameCloud.UCenter.Database.Entities;
using GameCloud.UCenter.Web.Common.Logger;
using GameCloud.UCenter.Web.Common.Storage;
using Microsoft.AspNetCore.Mvc;
using Pingpp.Models;

namespace GameCloud.UCenter.Api.ApiControllers
{
    /// <summary>
    /// UCenter payment api controller
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PaymentApiController : ApiControllerBase
    {
        private readonly Settings settings;
        private readonly StorageAccountContext storageContext;
        private readonly EventTrace eventTrace;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentApiController" /> class.
        /// </summary>
        /// <param name="database">The database context.</param>
        /// <param name="settings">The UCenter settings.</param>
        /// <param name="storageContext">The storage account context.</param>
        /// <param name="eventTrace">The event trace instance.</param>
        [ImportingConstructor]
        public PaymentApiController(
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
        /// Create a payment charge
        /// </summary>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        [HttpPost]
        [Route("api/payments")]
        public async Task<IActionResult> CreateCharge([FromBody]PaymentInfo info, CancellationToken token)
        {
            Dictionary<String, String> app = new Dictionary<String, String>();
            app.Add("id", settings.PingPlusPlusAppId);
            Dictionary<String, Object> param = new Dictionary<String, Object>();
            param.Add("order_no", info.OrderNo);
            param.Add("amount", info.Amount);
            param.Add("channel", info.Channel);
            param.Add("currency", "cny");
            param.Add("subject", info.Subject);
            param.Add("body", info.Body);
            param.Add("client_ip", "127.0.0.1");
            param.Add("app", app);
            try
            {
                //await CreateOrderAsync(token);
                var orderEntity = new OrderEntity
                {
                    Id = info.OrderNo,
                    Amount = info.Amount,
                    Channel = info.Channel,
                    Currency = info.Currency,
                    Subject = info.Subject,
                    Body = info.Body,
                    ClientIp = "",
                    CreatedTime = DateTime.UtcNow
                };

                await this.Database.Orders.InsertAsync(orderEntity, token);

                Charge charge = Charge.Create(param);

                return Ok(charge);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Create a payment charge
        /// </summary>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        [HttpPost]
        [Route("api/payments/callback")]
        public async Task<IActionResult> PaymentWebhook([FromBody]CallbackInfo info, CancellationToken token)
        {
            //获取 header 中的签名
            //string sig = Request.Headers.Get("x-pingplusplus-signature");

            //公钥路径（请检查你的公钥 .pem 文件存放路径）
            string path = @"C:\cert\pingpp\public.txt";

            //验证签名
            //string result = VerifySignedHash(inputData, sig, path);

            await UpdateOrderAsync(info, token);
            if (info.EventType.ToString() == "charge.succeeded" || info.EventType.ToString() == "refund.succeeded")
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        private async Task CreateOrderAsync(CancellationToken token)
        {
            var orderEntity = new OrderEntity
            {
                Id = Guid.NewGuid().ToString(),
                CreatedTime = DateTime.UtcNow
            };

            await this.Database.Orders.InsertAsync(orderEntity, token);
        }

        private async Task UpdateOrderAsync(CallbackInfo info, CancellationToken token)
        {
            var orderEntity = await this.Database.Orders.GetSingleAsync(o => o.Id == info.Id, token) ?? new OrderEntity
            {
                Id = info.Id,
                CreatedTime = DateTime.UtcNow
            };

            //order.State = eventType.ToString() == "charge.succeeded" || eventType.ToString() == "refund.succeeded"
            //    ? OrderState.Success
            //    : OrderState.Failed;

            //order.Content = orderData;
            //order.CompletedTime = DateTime.UtcNow;

            await this.Database.Orders.UpsertAsync(orderEntity, token);
        }

        private string VerifySignedHash(string str_DataToVerify, string str_SignedData, string str_publicKeyFilePath)
        {
            byte[] SignedData = Convert.FromBase64String(str_SignedData);

            ASCIIEncoding ByteConverter = new ASCIIEncoding();
            byte[] DataToVerify = ByteConverter.GetBytes(str_DataToVerify);
            try
            {
                string sPublicKeyPEM = System.IO.File.ReadAllText(str_publicKeyFilePath);
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

                rsa.PersistKeyInCsp = false;
                rsa.LoadPublicKeyPEM(sPublicKeyPEM);

                if (rsa.VerifyData(DataToVerify, "SHA256", SignedData))
                {
                    return "verify success";
                }
                else
                {
                    return "verify fail";
                }
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
                return "verify error";
            }
        }

    }
}