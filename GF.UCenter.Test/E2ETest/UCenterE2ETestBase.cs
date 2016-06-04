namespace GF.UCenter.Test
{
    using System.Threading.Tasks;
    using Common;
    using Common.Portable;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SDK.AppClient;
    using Web.Common.Logger;

    [TestClass]
    public class UCenterE2ETestBase : UCenterTestBase
    {
        protected const string TestAppId = "utapp";
        protected const string TestAppSecret = "#pA554&3321#";
        protected const string TestAppConfiguration = @"{foo:1,bar:2}";
        protected const string TestAppAccountData = @"{foo:1,bar:2}";

        protected const string InvalidAppSecret = "";
        protected const string ValidAccountPassword = "#pA554&3321#";
        protected const string InValidAccountPassword = "";
        protected const string InValidAccountToken = "";

        protected readonly string host;
        protected UCenterClient cClient;
        protected SDK.AppServer.UCenterClient sClient;

        public UCenterE2ETestBase()
            : base()
        {
            this.host = "http://localhost:8888";
            var settings = ExportProvider.GetExportedValue<Settings>();
            this.cClient = new UCenterClient($"http://{settings.ServerHost}:{settings.ServerPort}");
            this.sClient = new SDK.AppServer.UCenterClient(host);
        }

        [TestInitialize]
        public void Initialize()
        {
            // use public async void Initialize() will never triggered
            this.InitializeAsync().Wait();
        }

        private async Task InitializeAsync()
        {
            var appInfo = new AppInfo
            {
                AppId = TestAppId,
                AppSecret = TestAppSecret,
                Configuration = TestAppConfiguration
            };

            await sClient.AppCreateAsync(appInfo);
        }

        protected async Task<AccountRegisterResponse> CreateTestAccount(AccountRegisterInfo info = null)
        {
            if (info == null)
            {
                info = new AccountRegisterInfo
                {
                    AccountName = "an-" + GenerateRandomString(),
                    Password = ValidAccountPassword,
                    SuperPassword = ValidAccountPassword,
                    Name = "n-" + GenerateRandomString(),
                    IdentityNum = "in-" + GenerateRandomString(),
                    PhoneNum = "p-" + GenerateRandomString(),
                    Email = GenerateRandomString() + "@test.com",
                    Sex = Sex.Female
                };
            }
            var registerResponse = await cClient.AccountRegisterAsync(info);
            Assert.IsNotNull(registerResponse.AccountId);
            Assert.AreEqual(registerResponse.AccountName, info.AccountName);
            Assert.AreEqual(registerResponse.IdentityNum, info.IdentityNum);
            Assert.AreEqual(registerResponse.Name, info.Name);
            Assert.AreEqual(registerResponse.PhoneNum, info.PhoneNum);
            Assert.AreEqual(registerResponse.Email, info.Email);
            Assert.AreEqual(registerResponse.Sex, info.Sex);
            Assert.IsNotNull(registerResponse.ProfileImage);
            Assert.IsNotNull(registerResponse.ProfileThumbnail);

            return registerResponse;
        }
    }
}