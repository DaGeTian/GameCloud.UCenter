using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GameCloud.UCenter.Common.Models.AppServer;
using GameCloud.UCenter.Common.Portable.Contracts;
using GameCloud.UCenter.Common.Portable.Models.AppClient;
using GameCloud.UCenter.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace GameCloud.UCenter.Test.E2ETest
{
    [TestClass]
    public class UCenterE2EAppClientTest : UCenterE2ETestBase
    {
        [TestMethod]
        public async Task E2E_AppClient_Register_And_Login_Test()
        {
            var registerResponse = await CreateTestAccount();

            var loginResponse = await acClient.AccountLoginAsync(new AccountLoginInfo
            {
                AccountName = registerResponse.AccountName,
                Password = ValidAccountPassword,
                Device = TestDevice
            });

            Assert.IsNotNull(loginResponse.AccountId);
            Assert.AreEqual(loginResponse.AccountName, registerResponse.AccountName);
            Assert.AreEqual(loginResponse.IdentityNum, registerResponse.IdentityNum);
            Assert.AreEqual(loginResponse.Name, registerResponse.Name);
            Assert.AreEqual(loginResponse.PhoneNum, registerResponse.PhoneNum);
            Assert.AreEqual(loginResponse.Email, registerResponse.Email);
            Assert.AreEqual(loginResponse.Gender, registerResponse.Gender);
            Assert.IsNotNull(loginResponse.ProfileImage);
            Assert.IsNotNull(loginResponse.ProfileThumbnail);
            Assert.IsNotNull(loginResponse.LastLoginDateTime);
            Assert.IsNotNull(loginResponse.Token);
        }

        [TestMethod]
        public async Task E2E_AppClient_Register_ParallelTest()
        {
            await Task.WhenAll(ParallelEnumerable.Range(0, 10)
                .Select(async idx =>
                {
                    var random = $"account.{GenerateRandomString()}.{idx.ToString()}";
                    var info = new AccountRegisterInfo
                    {
                        AccountName = random,
                        Password = ValidAccountPassword,
                        SuperPassword = ValidAccountPassword,
                        Name = random,
                        Email = random,
                        IdentityNum = random,
                        PhoneNum = random,
                        Gender = Gender.Female,
                        Device = TestDevice
                    };
                    await this.CreateTestAccount(info);
                }));
        }

        [TestMethod]
        public async Task E2E_AppClient_Login_Incorrect_Password_Test()
        {
            var registerResponse = await CreateTestAccount();

            await TestExpector.ExpectUCenterErrorAsync(UCenterErrorCode.AccountLoginFailedPasswordNotMatch, async () =>
             {
                 await acClient.AccountLoginAsync(new AccountLoginInfo
                 {
                     AccountName = registerResponse.AccountName,
                     Password = InValidAccountPassword,
                     Device = TestDevice
                 });
             });
        }

        [TestMethod]
        public async Task E2E_AppClient_Register_WithInvalidChars_Test()
        {
            var info = new AccountRegisterInfo
            {
                Password = ValidAccountPassword,
                SuperPassword = ValidAccountPassword,
                Name = GenerateRandomString(),
                IdentityNum = GenerateRandomString(),
                PhoneNum = GenerateRandomString(),
                Email = GenerateRandomString() + "@test.com",
                Gender = Gender.Female,
                Device = TestDevice
            };

            // TOOD: Change ErrorCode in next client refresh
            info.AccountName = "$%^";
            await TestExpector.ExpectUCenterErrorAsync(UCenterErrorCode.AccountRegisterFailedAlreadyExist,
                async () => { await acClient.AccountRegisterAsync(info); });

            info.AccountName = "张无忌";
            await TestExpector.ExpectUCenterErrorAsync(UCenterErrorCode.AccountRegisterFailedAlreadyExist,
                async () => { await acClient.AccountRegisterAsync(info); });
        }

        [TestMethod]
        public async Task E2E_AppClient_Register_Twice_Test()
        {
            var info = new AccountRegisterInfo
            {
                AccountName = GenerateRandomString(),
                Password = ValidAccountPassword,
                SuperPassword = ValidAccountPassword,
                Name = $"account.{GenerateRandomString()}",
                IdentityNum = GenerateRandomString(),
                PhoneNum = GenerateRandomString(),
                Email = GenerateRandomString() + "@test.com",
                Gender = Gender.Female,
                Device = TestDevice
            };

            await acClient.AccountRegisterAsync(info);

            await TestExpector.ExpectUCenterErrorAsync(UCenterErrorCode.AccountRegisterFailedAlreadyExist,
                async () => { await acClient.AccountRegisterAsync(info); });
        }

        [TestMethod]
        public async Task E2E_AppClient_Guest_Login_And_Convert_Test()
        {
            var loginResponse = await acClient.AccountGuestLoginAsync(TestDevice);

            Assert.IsNotNull(loginResponse.AccountId);
            Assert.IsNotNull(loginResponse.AccountName);
            Assert.IsNotNull(loginResponse.Password);
            Assert.IsNotNull(loginResponse.Token);

            var convertInfo = new AccountConvertInfo
            {
                AccountId = loginResponse.AccountId,
                AccountName = $"guest.{GenerateRandomString()}",
                OldPassword = loginResponse.Password,
                Password = ValidAccountPassword,
                SuperPassword = ValidAccountPassword,
                Name = GenerateRandomString(),
                PhoneNum = GenerateRandomString(),
                Email = GenerateRandomString(),
                IdentityNum = GenerateRandomString(),
                Gender = Gender.Female
            };

            var convertResponse = await acClient.AccountConvertAsync(convertInfo);

            Assert.IsNotNull(convertResponse.AccountId);
            Assert.IsNotNull(convertResponse.AccountId, convertInfo.AccountId);
            Assert.AreEqual(convertResponse.AccountName, convertInfo.AccountName);
            Assert.AreEqual(convertResponse.IdentityNum, convertInfo.IdentityNum);
            Assert.AreEqual(convertResponse.Name, convertInfo.Name);
            Assert.AreEqual(convertResponse.PhoneNum, convertInfo.PhoneNum);
            Assert.AreEqual(convertResponse.Email, convertInfo.Email);
            Assert.AreEqual(convertResponse.Gender, convertInfo.Gender);
        }

        [TestMethod]
        public async Task E2E_AppClient_Reset_Password_Test()
        {
            var registerResponse = await CreateTestAccount();

            var resetInfo = new AccountResetPasswordInfo
            {
                AccountName = registerResponse.AccountName,
                Password = "123456",
                SuperPassword = ValidAccountPassword
            };

            await Task.Delay(TimeSpan.FromSeconds(1));

            var resetPasswordResponse = await acClient.AccountResetPasswordAsync(resetInfo);

            var loginInfo = new AccountLoginInfo
            {
                AccountName = registerResponse.AccountName,
                Password = ValidAccountPassword,
                Device = TestDevice
            };

            await TestExpector.ExpectUCenterErrorAsync(UCenterErrorCode.AccountLoginFailedPasswordNotMatch,
                async () => { await acClient.AccountLoginAsync(loginInfo); });
        }

        [TestMethod]
        public async Task E2E_AppClient_Upload_Profile_Image_By_Path_Test()
        {
            var registerResponse = await CreateTestAccount();

            var testFileForUpload = @"TestData\github.png";
            var uploadProfileResponse =
                await acClient.AccountUploadProfileImagesync(registerResponse.AccountId, testFileForUpload);
            Assert.AreEqual(registerResponse.AccountId, uploadProfileResponse.AccountId);
            Assert.AreEqual(registerResponse.AccountName, uploadProfileResponse.AccountName);
            Assert.AreEqual(registerResponse.Email, uploadProfileResponse.Email);
            Assert.AreEqual(registerResponse.IdentityNum, uploadProfileResponse.IdentityNum);
            Assert.AreEqual(registerResponse.Name, uploadProfileResponse.Name);
            Assert.AreEqual(registerResponse.PhoneNum, uploadProfileResponse.PhoneNum);
            Assert.AreEqual(registerResponse.Gender, uploadProfileResponse.Gender);
            Assert.IsNotNull(uploadProfileResponse.ProfileImage);
        }

        [TestMethod]
        public async Task E2E_AppClient_Upload_Profile_Image_By_Stream_Test()
        {
            var registerResponse = await CreateTestAccount();

            var testFileForUpload = @"TestData\github.png";
            using (var stream = new FileStream(testFileForUpload, FileMode.Open))
            {
                var uploadProfileResponse =
                    await acClient.AccountUploadProfileImagesync(registerResponse.AccountId, stream);
                Assert.AreEqual(registerResponse.AccountId, uploadProfileResponse.AccountId);
                Assert.AreEqual(registerResponse.AccountName, uploadProfileResponse.AccountName);
                Assert.AreEqual(registerResponse.Email, uploadProfileResponse.Email);
                Assert.AreEqual(registerResponse.IdentityNum, uploadProfileResponse.IdentityNum);
                Assert.AreEqual(registerResponse.Name, uploadProfileResponse.Name);
                Assert.AreEqual(registerResponse.PhoneNum, uploadProfileResponse.PhoneNum);
                Assert.AreEqual(registerResponse.Gender, uploadProfileResponse.Gender);
                Assert.IsNotNull(uploadProfileResponse.ProfileImage);
            }
        }

        [TestMethod]
        public async Task E2E_AppClient_GetAppConfiguration_Test()
        {
            var conf = new TestAppConfiguration()
            {
                Foo = "foo",
                Bar = "bar"
            };
            var appConfigurationInfo = new AppConfigurationInfo()
            {
                AppId = TestAppId,
                Configuration = JsonConvert.SerializeObject(conf)
            };
            await asClient.CreateAppConfigurationAsync(appConfigurationInfo);
            var result = await acClient.GetAppConfigurationAsync(TestAppId);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.AppId, TestAppId);
            CheckEquals(conf, JsonConvert.DeserializeObject<TestAppConfiguration>(result.Configuration));
        }

        private void CheckEquals(TestAppConfiguration conf1, TestAppConfiguration conf2)
        {
            Assert.AreEqual(conf1.Foo, conf2.Foo);
            Assert.AreEqual(conf1.Bar, conf2.Bar);
        }
    }
}