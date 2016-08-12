using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GameCloud.UCenter.Common.Models.AppServer;
using GameCloud.UCenter.Common.Models.PingPlusPlus;
using GameCloud.UCenter.Common.Portable.Contracts;
using GameCloud.UCenter.Common.Portable.Models.AppClient;
using GameCloud.UCenter.Test.Common;
using GameCloud.UCenter.Web.Api.ApiControllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameCloud.UCenter.Test.E2ETest
{
    [TestClass]
    public class UCenterE2EAppServerTest : UCenterE2ETestBase
    {
        [TestMethod]
        public async Task E2E_AppServer_AccountLoginApp_Test()
        {
            var registerResponse = await CreateTestAccount();

            var loginResponse = await acClient.AccountLoginAsync(new AccountLoginInfo
            {
                AccountName = registerResponse.AccountName,
                Password = ValidAccountPassword
            });

            var accountLoginAppInfo = new AccountLoginAppInfo
            {
                AppId = TestAppId,
                AppSecret = TestAppSecret,
                AccountId = loginResponse.AccountId,
                AccountToken = loginResponse.Token
            };
            var result = await asClient.AccountLoginAppAsync(accountLoginAppInfo);
            Assert.IsNotNull(result.AccountId);
            Assert.IsNotNull(result.AccountName);
            Assert.IsNotNull(result.AccountToken);
            Assert.IsNotNull(result.LastLoginDateTime);
            Assert.IsNotNull(result.LastVerifyDateTime);
        }

        [TestMethod]
        public async Task E2E_AppServer_AccountLoginApp_AccountNotExist_Test()
        {
            var accountLoginAppInfo = new AccountLoginAppInfo
            {
                AppId = TestAppId,
                AppSecret = TestAppSecret,
                AccountId = "___Test_Account_No_Exist___",
                AccountToken = ValidAccountPassword
            };

            await TestExpector.ExpectUCenterErrorAsync(UCenterErrorCode.AccountNotExist,
                async () => { await asClient.AccountLoginAppAsync(accountLoginAppInfo); });
        }

        [TestMethod]
        public async Task E2E_AppServer_AccountLoginApp_AppNotExist_Test()
        {
            var registerResponse = await CreateTestAccount();

            var loginResponse = await acClient.AccountLoginAsync(new AccountLoginInfo
            {
                AccountName = registerResponse.AccountName,
                Password = ValidAccountPassword
            });

            var accountLoginAppInfo = new AccountLoginAppInfo
            {
                AppId = "___Test_App_No_Exist___",
                AppSecret = TestAppSecret,
                AccountId = registerResponse.AccountId,
                AccountToken = loginResponse.Token
            };

            await TestExpector.ExpectUCenterErrorAsync(UCenterErrorCode.AppNotExit,
                async () => { await asClient.AccountLoginAppAsync(accountLoginAppInfo); });
        }

        [TestMethod]
        public async Task E2E_AppServer_AccountLoginApp_IncorrectAppSecret_Test()
        {
            var registerResponse = await CreateTestAccount();

            var loginResponse = await acClient.AccountLoginAsync(new AccountLoginInfo
            {
                AccountName = registerResponse.AccountName,
                Password = ValidAccountPassword
            });

            var accountLoginAppInfo = new AccountLoginAppInfo
            {
                AppId = TestAppId,
                AppSecret = InvalidAppSecret,
                AccountId = loginResponse.AccountId,
                AccountToken = ValidAccountPassword
            };

            await TestExpector.ExpectUCenterErrorAsync(UCenterErrorCode.AppAuthFailedSecretNotMatch,
                async () => { await asClient.AccountLoginAppAsync(accountLoginAppInfo); });
        }

        [TestMethod]
        public async Task E2E_AppServer_AccountLoginApp_IncorrectAccountToken_Test()
        {
            var registerResponse = await CreateTestAccount();

            var loginResponse = await acClient.AccountLoginAsync(new AccountLoginInfo
            {
                AccountName = registerResponse.AccountName,
                Password = ValidAccountPassword
            });

            var accountLoginAppInfo = new AccountLoginAppInfo
            {
                AppId = TestAppId,
                AppSecret = TestAppSecret,
                AccountId = loginResponse.AccountId,
                AccountToken = InValidAccountToken
            };

            await TestExpector.ExpectUCenterErrorAsync(UCenterErrorCode.AccountLoginFailedTokenNotMatch,
                async () => { await asClient.AccountLoginAppAsync(accountLoginAppInfo); });
        }

        [TestMethod]
        public async Task E2E_AppServer_ReadAccountData_And_WriteAccountData_Test()
        {
            var registerResponse = await CreateTestAccount();

            var loginResponse = await acClient.AccountLoginAsync(new AccountLoginInfo
            {
                AccountName = registerResponse.AccountName,
                Password = ValidAccountPassword
            });

            var data = @"{ 'id': 1, 'name': 'abc'}";
            var accountData = new AppAccountDataInfo
            {
                AppId = TestAppId,
                AppSecret = TestAppSecret,
                AccountId = loginResponse.AccountId,
                Data = data
            };

            await asClient.AppWriteAccountDataAsync(accountData);

            var result = await asClient.AppReadAccountDataAsync(accountData);

            Assert.AreEqual(accountData.AppId, result.AppId);
            Assert.AreEqual(accountData.AccountId, result.AccountId);
            Assert.AreEqual(accountData.Data, result.Data);
        }

        [TestMethod]
        public async Task E2E_AppServer_ReadAccountData_IncorrectAppSecret_Test()
        {
            var registerResponse = await CreateTestAccount();

            var loginResponse = await acClient.AccountLoginAsync(new AccountLoginInfo
            {
                AccountName = registerResponse.AccountName,
                Password = ValidAccountPassword
            });

            var accountData = new AppAccountDataInfo
            {
                AppId = TestAppId,
                AppSecret = InvalidAppSecret,
                AccountId = loginResponse.AccountId
            };

            await TestExpector.ExpectUCenterErrorAsync(UCenterErrorCode.AppAuthFailedSecretNotMatch,
                async () => { await asClient.AppReadAccountDataAsync(accountData); });
        }

        [TestMethod]
        public async Task E2E_AppServer_WriteAccountData_InvalidAppSecret_Test()
        {
            var registerResponse = await CreateTestAccount();

            var loginResponse = await acClient.AccountLoginAsync(new AccountLoginInfo
            {
                AccountName = registerResponse.AccountName,
                Password = ValidAccountPassword
            });

            var data = @"{ 'id': 1, 'name': 'abc'}";
            var accountData = new AppAccountDataInfo
            {
                AppId = TestAppId,
                AppSecret = InvalidAppSecret,
                AccountId = loginResponse.AccountId,
                Data = data
            };

            await TestExpector.ExpectUCenterErrorAsync(UCenterErrorCode.AppAuthFailedSecretNotMatch,
               async () => { await asClient.AppWriteAccountDataAsync(accountData); });
        }

        [TestMethod]
        public async Task E2E_AppServer_Create_Order_Test()
        {
            var registerResponse = await CreateTestAccount();

            var loginResponse = await acClient.AccountLoginAsync(new AccountLoginInfo
            {
                AccountName = registerResponse.AccountName,
                Password = ValidAccountPassword
            });

            var chargeInfo = new ChargeInfo
            {
                AppId = TestAppId,
                AppSecret = TestAppSecret,
                AccountId = loginResponse.AccountId,
                Amount = 100,
                Subject = "Super Axe",
                Body = "Test body",
                ClientIp = "1.2.3.4",
                Description = "This is a test order created by unit test"
            };

            var result = await asClient.CreateChargeAsync(chargeInfo);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Amount, chargeInfo.Amount);
            Assert.AreEqual(result.Subject, chargeInfo.Subject);
            Assert.AreEqual(result.Body, chargeInfo.Body);
            //// Assert.AreEqual(result.Description, chargeInfo.Description);
            Assert.IsNotNull(result.OrderNo);
            //// Assert.IsNotNull(result.TransactionNo);
        }

        [TestMethod]
        public async Task E2E_AppServer_Complete_Order_Test()
        {
            var controller = ExportProvider.GetExportedValue<PaymentApiController>();
            string orderData = File.ReadAllText(@"TestData\charge.succeeded.json");
            await controller.ProcessOrderAsync(orderData, CancellationToken.None);
        }
    }
}