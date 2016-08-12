using System;
using System.Threading;
using System.Threading.Tasks;
using GameCloud.UCenter;
using GameCloud.UCenter.Test.E2ETest;
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
    }
}