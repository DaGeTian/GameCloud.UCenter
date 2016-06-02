namespace GF.UCenter.Test.MongoDB
{
    using UCenter.MongoDB.Repository;
    using UCenter.MongoDB.Entity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AppRepositoryTest : RepositoryTestBase
    {
        private readonly IAppRepository appRepository;

        public AppRepositoryTest()
        {
            appRepository = new AppRepository(MongoContext);
        }

        [TestMethod]
        public void App_Add_Test()
        {
            string appName = GenerateRandomString();
            var appEntity = new App
            {
                Id = TestAppId,
                Name = appName,
                Configuration = TestAppConfiguration
            };
            var response = appRepository.Add(appEntity);
            Assert.IsNotNull(response);
            Assert.AreEqual(response.Name, appName);
            Assert.AreEqual(response.Configuration, TestAppConfiguration);
        }

        [TestMethod]
        public void App_Update_Test()
        {
            string appNewName = "TestAppNewName";
            var appEntitry = appRepository.GetById(TestAppId);
            appEntitry.Name = appNewName;
            var response = appRepository.Update(appEntitry);
            Assert.IsNotNull(response);
            Assert.AreEqual(response.Name, appNewName);
        }

        [TestMethod]
        public void App_Delete_Test()
        {
            appRepository.Delete(TestAppId);
            var appEntitry = appRepository.GetById(TestAppId);
            Assert.IsNull(appEntitry);
        }
    }
}
