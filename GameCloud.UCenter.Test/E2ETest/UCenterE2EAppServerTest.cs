using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GameCloud.UCenter.Common.Models.AppServer;
using GameCloud.UCenter.Common.Portable.Contracts;
using GameCloud.UCenter.Common.Portable.Models.AppClient;
using GameCloud.UCenter.Test.Common;
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
                Password = ValidAccountPassword,
                Device = TestDevice
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

            await TestExpector.ExpectUCenterErrorAsync(
                UCenterErrorCode.AccountNotExist,
                async () => { await asClient.AccountLoginAppAsync(accountLoginAppInfo); });
        }

        [TestMethod]
        public async Task E2E_AppServer_AccountLoginApp_AppNotExist_Test()
        {
            var registerResponse = await CreateTestAccount();

            var loginResponse = await acClient.AccountLoginAsync(new AccountLoginInfo
            {
                AccountName = registerResponse.AccountName,
                Password = ValidAccountPassword,
                Device = TestDevice
            });

            var accountLoginAppInfo = new AccountLoginAppInfo
            {
                AppId = "___Test_App_No_Exist___",
                AppSecret = TestAppSecret,
                AccountId = registerResponse.AccountId,
                AccountToken = loginResponse.Token
            };

            await TestExpector.ExpectUCenterErrorAsync(
                UCenterErrorCode.AppNotExists,
                async () => { await asClient.AccountLoginAppAsync(accountLoginAppInfo); });
        }

        [TestMethod]
        public async Task E2E_AppServer_AccountLoginApp_IncorrectAppSecret_Test()
        {
            var registerResponse = await CreateTestAccount();

            var loginResponse = await acClient.AccountLoginAsync(new AccountLoginInfo
            {
                AccountName = registerResponse.AccountName,
                Password = ValidAccountPassword,
                Device = TestDevice
            });

            var accountLoginAppInfo = new AccountLoginAppInfo
            {
                AppId = TestAppId,
                AppSecret = InvalidAppSecret,
                AccountId = loginResponse.AccountId,
                AccountToken = ValidAccountPassword
            };

            await TestExpector.ExpectUCenterErrorAsync(
                UCenterErrorCode.AppTokenUnauthorized,
                async () => { await asClient.AccountLoginAppAsync(accountLoginAppInfo); });
        }

        [TestMethod]
        public async Task E2E_AppServer_AccountLoginApp_IncorrectAccountToken_Test()
        {
            var registerResponse = await CreateTestAccount();

            var loginResponse = await acClient.AccountLoginAsync(new AccountLoginInfo
            {
                AccountName = registerResponse.AccountName,
                Password = ValidAccountPassword,
                Device = TestDevice
            });

            var accountLoginAppInfo = new AccountLoginAppInfo
            {
                AppId = TestAppId,
                AppSecret = TestAppSecret,
                AccountId = loginResponse.AccountId,
                AccountToken = InValidAccountToken
            };

            await TestExpector.ExpectUCenterErrorAsync(
                UCenterErrorCode.AccountTokenUnauthorized,
                async () => { await asClient.AccountLoginAppAsync(accountLoginAppInfo); });
        }

        [TestMethod]
        public async Task E2E_AppServer_ReadAccountData_And_WriteAccountData_Test()
        {
            var registerResponse = await CreateTestAccount();

            var loginResponse = await acClient.AccountLoginAsync(new AccountLoginInfo
            {
                AccountName = registerResponse.AccountName,
                Password = ValidAccountPassword,
                Device = TestDevice
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
                Password = ValidAccountPassword,
                Device = TestDevice
            });

            var accountData = new AppAccountDataInfo
            {
                AppId = TestAppId,
                AppSecret = InvalidAppSecret,
                AccountId = loginResponse.AccountId
            };

            await TestExpector.ExpectUCenterErrorAsync(
                UCenterErrorCode.AppTokenUnauthorized,
                async () => { await asClient.AppReadAccountDataAsync(accountData); });
        }

        [TestMethod]
        public async Task E2E_AppServer_WriteAccountData_InvalidAppSecret_Test()
        {
            var registerResponse = await CreateTestAccount();

            var loginResponse = await acClient.AccountLoginAsync(new AccountLoginInfo
            {
                AccountName = registerResponse.AccountName,
                Password = ValidAccountPassword,
                Device = TestDevice
            });

            var data = @"{ 'id': 1, 'name': 'abc'}";
            var accountData = new AppAccountDataInfo
            {
                AppId = TestAppId,
                AppSecret = InvalidAppSecret,
                AccountId = loginResponse.AccountId,
                Data = data
            };

            await TestExpector.ExpectUCenterErrorAsync(
                UCenterErrorCode.AppTokenUnauthorized,
                async () => { await asClient.AppWriteAccountDataAsync(accountData); });
        }
    }
}