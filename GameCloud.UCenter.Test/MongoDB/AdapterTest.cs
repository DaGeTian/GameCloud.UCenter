using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GameCloud.Database.Adapters;
using GameCloud.UCenter.Common.Portable.Models.AppClient;
using GameCloud.UCenter.Database.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GameCloud.UCenter.Test.MongoDB
{
    [TestClass]
    public class AdapterTest : UCenterTestBase
    {
        [TestMethod]
        public async Task Adapter_EntityCRUD_Test()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            var adapter = ExportProvider.GetExportedValue<ICollectionAdapter<AccountEntity>>();

            string accountNameBeforeUpdate = GenerateRandomString();
            string accountNameAfterUpdate = GenerateRandomString();

            var accountEntity = new AccountEntity
            {
                Id = Guid.NewGuid().ToString(),
                AccountName = accountNameBeforeUpdate,
                AccountType = AccountType.NormalAccount,
                Name = GenerateRandomString(),
                CreatedTime = DateTime.UtcNow,
                Email = "abc@ab.com",
                Phone = "12345678",
                Gender = Gender.Male
            };

            await adapter.InsertAsync(accountEntity, token);

            var entityFromServer = await adapter.GetSingleAsync(accountEntity.Id, token);
            this.CheckEquals(accountEntity, entityFromServer);

            var filter = Builders<AccountEntity>.Filter.Where(e => e.Id == accountEntity.Id);
            var update = Builders<AccountEntity>.Update.Set("Name", accountNameAfterUpdate);
            await adapter.UpdateOneAsync(accountEntity, filter, update, token);

            var list = await adapter.GetListAsync(e => e.Name == accountNameBeforeUpdate, token);
            Assert.AreEqual(0, list.Count);

            list = await adapter.GetListAsync(e => e.Name == accountNameAfterUpdate, token);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(accountNameAfterUpdate, list.First().Name);

            await adapter.DeleteAsync(accountEntity, token);

            list = await adapter.GetListAsync(e => e.AccountName == accountNameAfterUpdate, token);

            Assert.AreEqual(0, list.Count);
        }

        private void CheckEquals(AccountEntity entity1, AccountEntity entity2)
        {
            Assert.AreEqual(entity1.Id, entity2.Id);
            Assert.AreEqual(entity1.AccountName, entity2.AccountName);
            Assert.AreEqual(entity1.AccountType, entity2.AccountType);
            Assert.AreEqual(entity1.AccountStatus, entity2.AccountStatus);
            Assert.AreEqual(entity1.Token, entity2.Token);
            Assert.AreEqual(entity1.Identity, entity2.Identity);
            Assert.AreEqual(entity1.Name, entity2.Name);
            Assert.AreEqual(entity1.Email, entity2.Email);
            Assert.AreEqual(entity1.Phone, entity2.Phone);
            Assert.AreEqual(entity1.Gender, entity2.Gender);
            Assert.AreEqual(entity1.ProfileImage, entity2.ProfileImage);
            Assert.AreEqual(entity1.ProfileThumbnail, entity2.ProfileThumbnail);
            Assert.AreEqual(entity1.LastLoginDateTime, entity2.LastLoginDateTime);
            Assert.AreEqual(entity1.CreatedTime.ToString("s"), entity2.CreatedTime.ToString("s"));
        }
    }
}
