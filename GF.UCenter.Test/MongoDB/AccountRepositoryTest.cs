namespace GF.UCenter.Test.MongoDB
{
    using UCenter.MongoDB.Entity;
    using UCenter.MongoDB.Repository;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AccountRepositoryTest : RepositoryTestBase
    {
        private IAccountRepository accountRepository;
        private string accountKey = "TestAccountKey";

        public AccountRepositoryTest()
        {
            accountRepository = new AccountRepository(MongoContext);
        }

        [TestMethod]
        public void Account_Add_Test()
        {
            string accountName = "TestAccountName";
            var accountEntity = new Account
            {
                Id = accountKey,
                Name = GenerateRandomString(),
            };
            var response = accountRepository.Add(accountEntity);
            Assert.IsNotNull(response.Id);
            Assert.AreEqual(response.Name, accountName);
        }

        [TestMethod]
        public void Account_Update_Test()
        {
            string accountNewName = "TestAccountNewName";
            var accountEntitry = accountRepository.GetById(accountKey);
            accountEntitry.Name = accountNewName;
            var response = accountRepository.Update(accountEntitry);
            Assert.IsNotNull(response.Name, accountNewName);
        }

        [TestMethod]
        public void Account_Delete_Test()
        {
            accountRepository.Delete(accountKey);
            var accountEntitry = accountRepository.GetById(accountKey);
            Assert.IsNull(accountEntitry);
        }
    }
}
