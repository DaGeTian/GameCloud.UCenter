namespace GF.UCenter.Test
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Common.IP;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EncryptHashManagerTest
    {
        [TestMethod]
        public void TestEncryptAndCompare()
        {
            string password = Guid.NewGuid().ToString();
            var hash = EncryptHashManager.ComputeHash(password);
            Assert.IsFalse(EncryptHashManager.VerifyHash(Guid.NewGuid().ToString(), hash));
            Assert.IsTrue(EncryptHashManager.VerifyHash(password, hash));
        }

        [TestMethod]
        public async Task TestGetIPInfo()
        {
            string ip = "23.99.99.89";
            var response = await IPHelper.GetIPInfoAsync("23.99.99.89", CancellationToken.None);
            Assert.AreEqual(response.Code, IPInfoResponseCode.Success);
            Assert.AreEqual(response.Info.IP, ip);
            Assert.AreEqual(response.Info.Country, "香港");
        }
    }
}