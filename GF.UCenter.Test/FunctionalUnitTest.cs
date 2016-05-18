namespace GF.UCenter.Test
{
    using System;
    using System.ComponentModel.Composition.Hosting;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Common.IP;
    using CouchBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Web;

    [TestClass]
    public class FunctionalUnitTest : UCenterE2ETestBase
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

        [TestMethod]
        public void TestQueryTranslator()
        {
            Expression<Func<AccountEntity, bool>> expression = a => a.AccountName.Contains("abc") || a.Name.Contains("abc");
            var translator = new CouchQueryTranslator();
            var command = translator.Translate(expression);

            Assert.AreEqual(command.Command, "accountName like $1  OR  name like $2");
            Assert.AreEqual(2, command.Parameters.Count);
            Assert.AreEqual("%abc%", command.Parameters.First().Value);
        }

        [TestMethod]
        public async Task TestProcessOrder()
        {
            var settings = ExportProvider.GetExportedValue<Common.Settings.Settings>();
            CouchBaseContext db = new CouchBaseContext(settings);
            var controller = new PaymentApiController(db, settings);
            string orderData = File.ReadAllText(@"TestData\charge.succeeded.json");
            await controller.ProcessOrderAsync(orderData);
        }
    }
}