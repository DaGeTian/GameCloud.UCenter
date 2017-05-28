using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameCloud.Database.Adapters;
using GameCloud.UCenter.Api.Extensions;
using GameCloud.UCenter.Common.Models.AppServer;
using GameCloud.UCenter.Common.Settings;
using GameCloud.UCenter.Common.Models.PingPlusPlus;
using GameCloud.UCenter.Common.Portable.Contracts;
using GameCloud.UCenter.Common.Portable.Exceptions;
using GameCloud.UCenter.Common.Portable.Models.AppClient;
using GameCloud.UCenter.Database;
using GameCloud.UCenter.Database.Entities;
using GameCloud.UCenter.Web.Common.Logger;
using Microsoft.AspNetCore.Mvc;
using Pingpp.Models;

namespace GameCloud.UCenter.Api.ApiControllers
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PaymentApiController : ApiControllerBase
    {
        //---------------------------------------------------------------------
        private readonly Settings settings;
        private readonly EventTrace eventTrace;

        //---------------------------------------------------------------------
        [ImportingConstructor]
        public PaymentApiController(
            UCenterDatabaseContext database,
            Settings settings,
            EventTrace eventTrace)
            : base(database)
        {
            this.settings = settings;
            this.eventTrace = eventTrace;
        }

        //---------------------------------------------------------------------
        [HttpPost]
        [Route("api/payments")]
        public async Task<IActionResult> CreateCharge([FromBody]PaymentInfo info, CancellationToken token)
        {
            OrderEntity orderEntity;
            string orderNumber = info.OrderNumber;
            if (string.IsNullOrEmpty(orderNumber))
            {
                orderNumber = Guid.NewGuid().ToString();
                orderEntity = new OrderEntity
                {
                    Id = orderNumber,
                    Amount = info.Amount,
                    Channel = info.Channel,
                    Currency = info.Currency,
                    Subject = info.Subject,
                    Body = info.Body,
                    ClientIp = "",
                    Status = OrderStatus.Created,
                    CreatedTime = DateTime.UtcNow
                };

                await this.Database.Orders.InsertAsync(orderEntity, token);
            }
            else
            {
                orderEntity = await this.Database.Orders.GetSingleAsync(o => o.Id == info.OrderNumber, token);
                if (orderEntity == null)
                {
                    throw new UCenterException(UCenterErrorCode.OrderNotExists, "Order not exists");
                }
            }

            var app = new Dictionary<string, string>
                {
                    {"id", settings.PingPlusPlusAppId}
                };
            var param = new Dictionary<string, object>
                {
                    {"order_no", orderNumber},
                    {"amount", info.Amount},
                    {"channel", info.Channel},
                    {"currency", "cny"},
                    {"subject", info.Subject},
                    {"body", info.Body},
                    {"client_ip", "127.0.0.1"},
                    {"app", app}
                };

            var charge = Pingpp.Models.Charge.Create(param);
            var response = new PaymentResponse()
            {
                AccountId = info.AccountId,
                // todo: reuse model
                //Charge = charge as Common.Portable.Models.AppClient.Charge
            };

            return this.CreateSuccessResult(response);
        }

        //---------------------------------------------------------------------
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

        //---------------------------------------------------------------------
        private async Task CreateOrderAsync(CancellationToken token)
        {
            var orderEntity = new OrderEntity
            {
                Id = Guid.NewGuid().ToString(),
                CreatedTime = DateTime.UtcNow
            };

            await this.Database.Orders.InsertAsync(orderEntity, token);
        }

        //---------------------------------------------------------------------
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

        //---------------------------------------------------------------------
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