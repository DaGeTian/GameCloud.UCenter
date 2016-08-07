namespace GF.UCenter.Test
{
    using System.Threading.Tasks;
    using Common;
    using Common.Portable.Models.AppClient;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SDK.AppClient;

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
            var settings = ExportProvider.GetExportedValue<Settings>();
            string host = $"http://{settings.ServerHost}:{settings.ServerPort}";
            this.acClient = new UCenterClient(host);
            this.asClient = new SDK.AppServer.UCenterClient(host);
        }

        [TestInitialize]
        public void Initialize()
        {
            // Note: Do not use public async void Initialize(), it will never triggered
            this.InitializeAsync().Wait();
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
                    IdentityNum = $"id.{GenerateRandomString()}",
                    PhoneNum = $"phone.{GenerateRandomString()}",
                    Email = $"{GenerateRandomString()}@test.com",
                    Gender = Gender.Female
                };
            }

            var registerResponse = await acClient.AccountRegisterAsync(info);
            Assert.IsNotNull(registerResponse.AccountId);
            Assert.AreEqual(registerResponse.AccountName, info.AccountName);
            Assert.AreEqual(registerResponse.AccountStatus, AccountStatus.Active);
            Assert.AreEqual(registerResponse.IdentityNum, info.IdentityNum);
            Assert.AreEqual(registerResponse.Name, info.Name);
            Assert.AreEqual(registerResponse.PhoneNum, info.PhoneNum);
            Assert.AreEqual(registerResponse.Email, info.Email);
            Assert.AreEqual(registerResponse.Gender, info.Gender);
            Assert.IsNotNull(registerResponse.ProfileImage);
            Assert.IsNotNull(registerResponse.ProfileThumbnail);

            return registerResponse;
        }

        private async Task InitializeAsync()
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