using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GameCloud.UCenter.Common;
using GameCloud.UCenter.Common.IP;
using GameCloud.UCenter.Test.E2ETest;
using GameCloud.UCenter.Web.Common.Storage;
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
        public async Task TestAzureStorageContext()
        {
            var context = ExportProvider.GetExportedValue<IStorageContext>("Storage.Azure");
            Assert.AreEqual(typeof(AzureStorageContext), context.GetType());

            using (var fileStream = File.OpenRead(@"TestData\github.png"))
            {
                string blobName = "testazure-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                await context.UploadBlobAsync(blobName, fileStream, CancellationToken.None);
            }
        }

        [TestMethod]
        public async Task TestAliyunOSSContext()
        {
            var context = ExportProvider.GetExportedValue<IStorageContext>("Storage.Ali");
            Assert.AreEqual(typeof(AliyunStorageContext), context.GetType());

            using (var fileStream = File.OpenRead(@"TestData\github.png"))
            {
                string blobName = "testazure-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                await context.UploadBlobAsync(blobName, fileStream, CancellationToken.None);
            }
        }
    }
}