using System.Threading.Tasks;
using GameCloud.UCenter.Common.Models.AppServer;
using GameCloud.UCenter.Common.Portable.Models.AppClient;
using GameCloud.UCenter.SDK.AppClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameCloud.UCenter.Test.E2ETest
{
    [TestClass]
    public class UCenterE2ETestBase : UCenterTestBase
    {
        protected const string TestAppId = "utapp";
        protected const string TestAppSecret = "#pA554&3321#";
        protected const string TestAppAccountData = @"{foo:1,bar:2}";
        protected const string InvalidAppSecret = "";
        protected const string ValidAccountPassword = "#pA554&3321#";
        protected const string InValidAccountPassword = "";
        protected const string InValidAccountToken = "";

        protected SDK.AppClient.UCenterClient acClient;
        protected SDK.AppServer.UCenterClient asClient;

        public UCenterE2ETestBase()
        {
            var settings = ExportProvider.GetExportedValue<UCenterTestSettings>();
            string host = $"http://{settings.UCenterServerHost}:{settings.UCenterServerPort}";
            this.acClient = new UCenterClient(host);
            this.asClient = new SDK.AppServer.UCenterClient(host);
        }

        protected DeviceInfo TestDevice => new DeviceInfo
        {
            Id = "UnitTestDeviceId",
            Name = "UnitTestDeviceName",
            Type = "UnitTestDeviceType",
            Model = "UnitTestDeviceType",
            OperationSystem = "UnitTestDeviceOS"
        };

        [ClassInitialize]
        public void Initialize()
        {
            // Note: Do not use public async void Initialize(), it will never triggered
            this.ClassInitializeAsync().Wait();
        }

        protected async Task<AccountRegisterResponse> CreateTestAccount(AccountRegisterInfo info = null)
        {
            if (info == null)
            {
                info = new AccountRegisterInfo
                {
                    AccountName = $"account.{GenerateRandomString()}",
                    Password = ValidAccountPassword,
                    SuperPassword = ValidAccountPassword,
                    Name = $"name.{GenerateRandomString()}",
                    Identity = $"id.{GenerateRandomString()}",
                    Phone = string.Empty,
                    Email = GenerateRandomString() + "@test.com",
                    Gender = Gender.Female,
                    Device = TestDevice
                };
            }

            var registerResponse = await acClient.AccountRegisterAsync(info);
            Assert.IsNotNull(registerResponse.AccountId);
            Assert.AreEqual(registerResponse.AccountName, info.AccountName);
            Assert.AreEqual(registerResponse.AccountType, AccountType.NormalAccount);
            Assert.AreEqual(registerResponse.AccountStatus, AccountStatus.Active);
            Assert.AreEqual(registerResponse.Identity, info.Identity);
            Assert.AreEqual(registerResponse.Name, info.Name);
            Assert.AreEqual(registerResponse.Phone, info.Phone);
            Assert.AreEqual(registerResponse.Email, info.Email);
            Assert.AreEqual(registerResponse.Gender, info.Gender);

            return registerResponse;
        }
        
        private async Task ClassInitializeAsync()
        {
            var appInfo = new AppInfo
            {
                AppId = TestAppId,
                AppSecret = TestAppSecret
            };

            await asClient.CreateAppAsync(appInfo);
        }
    }
}