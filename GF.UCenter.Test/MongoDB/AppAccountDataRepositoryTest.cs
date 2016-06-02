
namespace GF.UCenter.Test.MongoDB
{
    using UCenter.MongoDB.Repository;
    using UCenter.MongoDB.Entity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AppAccountDataRepositoryTest : RepositoryTestBase
    {
        private readonly IAppAccountDataRepository appAccountDataRepository;
        private string appAccountDataKey = "TestAppAccountDataAccountDataKey";

        public AppAccountDataRepositoryTest()
        {
            appAccountDataRepository = new AppAccountDataRepository(MongoContext);
        }

        [TestMethod]
        public void AppAccountData_Add_Test()
        {
            var appAccountDataEntity = new AppAccountData
            {
                Id = appAccountDataKey,
                Data = TestAppAccountData
            };
            var response = appAccountDataRepository.Add(appAccountDataEntity);
            Assert.IsNotNull(response.Id);
        }

        [TestMethod]
        public void AppAccountData_Update_Test()
        {
            string appNewName = "TestAppAccountDataNewName";
            var appAccountDataEntitry = appAccountDataRepository.GetById(appAccountDataKey);
            appAccountDataEntitry.AppId = TestAppId;
            var response = appAccountDataRepository.Update(appAccountDataEntitry);
            Assert.AreEqual(response.AppId, TestAppId);
        }

        [TestMethod]
        public void AppAccountData_Delete_Test()
        {
            appAccountDataRepository.Delete(TestAppId);
            var appEntitry = appAccountDataRepository.GetById(TestAppId);
            Assert.IsNull(appEntitry);
        }
    }
}
