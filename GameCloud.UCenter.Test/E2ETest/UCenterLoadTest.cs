using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GameCloud.UCenter.Common.Models.AppServer;
using GameCloud.UCenter.Common.Portable.Contracts;
using GameCloud.UCenter.Common.Portable.Models.AppClient;
using GameCloud.UCenter.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace GameCloud.UCenter.Test.E2ETest
{
    [TestClass]
    public class UCenterLoadTest : UCenterE2ETestBase
    {
        // Test in this class is only for loading test

        // [TestMethod]
        public async Task LoadTest_GetAppConfiguration()
        {
            var result = await acClient.GetAppConfigurationAsync(TestAppId);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.AppId, TestAppId);
        }

        // [TestMethod]
        public async Task LoadTest_Login()
        {
            var loginResponse = await acClient.AccountLoginAsync(new AccountLoginInfo
            {
                AccountName = "Test10000",
                Password = "123456",
                Device = TestDevice
            });

            Assert.IsNotNull(loginResponse.AccountId);
            Assert.AreEqual(loginResponse.AccountName, "Test10000");
        }
    }
}