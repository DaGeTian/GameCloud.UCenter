using System;
using System.Threading;
using System.Threading.Tasks;
using GameCloud.UCenter.Common;
using GameCloud.UCenter.Common.IP;
using GameCloud.UCenter.Common.Portable.Models.Ip;
using GameCloud.UCenter.Test.E2ETest;
using GameCloud.UCenter.Web.Common.Logger;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameCloud.UCenter.Test
{
    [TestClass]
    public class FunctionalUnitTest : UCenterE2ETestBase
    {
        [TestMethod]
        public void TestEncryptAndCompare()
        {
            string password = Guid.NewGuid().ToString();
            var hash = EncryptHelper.SHA256(password);
            Assert.IsTrue(EncryptHelper.VerifyHash(password, hash));
            Assert.IsFalse(EncryptHelper.VerifyHash(Guid.NewGuid().ToString(), hash));
        }

        [TestMethod]
        public async Task TestGetIPInfo()
        {
            string ip = "23.99.99.89";
            var response = await IPHelper.GetIPInfoAsync("23.99.99.89", CancellationToken.None);
            Assert.AreEqual(response.Code, IPInfoResponseCode.Success);
            Assert.AreEqual(response.Content.IP, ip);
            Assert.AreEqual(response.Content.Country, "香港");
        }
    }
}