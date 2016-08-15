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
        public async Task E2E_AppClient_GetIpAddress_Test()
        {
            string ip = await acClient.GetCleintIpAddressAsync();
            Assert.IsNotNull(ip);
        }

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
            Assert.AreEqual(loginResponse.Identity, registerResponse.Identity);
            Assert.AreEqual(loginResponse.Name, registerResponse.Name);
            Assert.AreEqual(loginResponse.Phone, registerResponse.Phone);
            Assert.AreEqual(loginResponse.Email, registerResponse.Email);
            Assert.AreEqual(loginResponse.Gender, registerResponse.Gender);
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
                        Identity = random,
                        Phone = random,
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

            await TestExpector.ExpectUCenterErrorAsync(
                UCenterErrorCode.AccountPasswordUnauthorized,
                async () =>
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
        public async Task E2E_AppClient_Register_InvalidAccountName_Test()
        {
            var info = new AccountRegisterInfo
            {
                AccountName = GenerateRandomString(),
                Password = ValidAccountPassword,
                SuperPassword = ValidAccountPassword,
                Name = GenerateRandomString(),
                Identity = GenerateRandomString(),
                Phone = GenerateRandomString(),
                Email = GenerateRandomString() + "@test.com",
                Gender = Gender.Female,
                Device = TestDevice
            };

            // Account name length < 4 or > 64
            info.AccountName = "ABC";
            await TestExpector.ExpectUCenterErrorAsync(
                UCenterErrorCode.InvalidAccountName,
                async () => { await acClient.AccountRegisterAsync(info); });

            info.AccountName = $"{GenerateRandomString()}-^$";
            await TestExpector.ExpectUCenterErrorAsync(
                UCenterErrorCode.InvalidAccountName,
                async () => { await acClient.AccountRegisterAsync(info); });

            info.AccountName = "张无忌";
            await TestExpector.ExpectUCenterErrorAsync(
                UCenterErrorCode.InvalidAccountName,
                async () => { await acClient.AccountRegisterAsync(info); });
        }

        [TestMethod]
        public async Task E2E_AppClient_Register_InvalidPassword_Test()
        {
            var info = new AccountRegisterInfo
            {
                AccountName = GenerateRandomString(),
                Password = ValidAccountPassword,
                SuperPassword = ValidAccountPassword,
                Name = GenerateRandomString(),
                Identity = GenerateRandomString(),
                Phone = GenerateRandomString(),
                Gender = Gender.Female,
                Device = TestDevice
            };

            // Account password length < 6 or > 64
            info.Password = "12345";
            await TestExpector.ExpectUCenterErrorAsync(
                UCenterErrorCode.InvalidAccountPassword,
                async () => { await acClient.AccountRegisterAsync(info); });

            info.Password = "1234567890123456789012345678901234567890123456789012345678901234567890";
            await TestExpector.ExpectUCenterErrorAsync(
                UCenterErrorCode.InvalidAccountPassword,
                async () => { await acClient.AccountRegisterAsync(info); });
        }

        [TestMethod]
        public async Task E2E_AppClient_Register_InvalidEmail_Test()
        {
            var info = new AccountRegisterInfo
            {
                AccountName = GenerateRandomString(),
                Password = ValidAccountPassword,
                SuperPassword = ValidAccountPassword,
                Name = GenerateRandomString(),
                Identity = GenerateRandomString(),
                Phone = GenerateRandomString(),
                Email = GenerateRandomString() + "@test.com",
                Gender = Gender.Female,
                Device = TestDevice
            };

            info.Email = "12345";
            await TestExpector.ExpectUCenterErrorAsync(
                UCenterErrorCode.InvalidAccountEmail,
                async () => { await acClient.AccountRegisterAsync(info); });
        }

        [TestMethod]
        public async Task E2E_AppClient_Register_Twice_Test()
        {
            string accountName = GenerateRandomString();
            string phone = GenerateRandomString();
            string email = GenerateRandomString() + "@test.com";

            var info = new AccountRegisterInfo
            {
                AccountName = accountName,
                Password = ValidAccountPassword,
                SuperPassword = ValidAccountPassword,
                Name = $"account.{GenerateRandomString()}",
                Identity = GenerateRandomString(),
                Phone = phone,
                Email = email,
                Gender = Gender.Female,
                Device = TestDevice
            };

            await acClient.AccountRegisterAsync(info);

            await TestExpector.ExpectUCenterErrorAsync(
                UCenterErrorCode.AccountNameAlreadyExist,
                async () => { await acClient.AccountRegisterAsync(info); });

            info.AccountName = GenerateRandomString();
            await TestExpector.ExpectUCenterErrorAsync(
                UCenterErrorCode.AccountNameAlreadyExist,
                async () => { await acClient.AccountRegisterAsync(info); });

            info.Phone = GenerateRandomString();
            await TestExpector.ExpectUCenterErrorAsync(
                UCenterErrorCode.AccountNameAlreadyExist,
                async () => { await acClient.AccountRegisterAsync(info); });
        }

        [TestMethod]
        public async Task E2E_AppClient_GuestAccess_And_Convert_Test()
        {
            var loginResponse = await acClient.AccountGuestAccessAsync(TestDevice);

            Assert.IsNotNull(loginResponse.AccountId);
            Assert.IsNotNull(loginResponse.AccountName);
            Assert.IsNotNull(loginResponse.Token);

            var convertInfo = new GuestConvertInfo
            {
                AccountId = loginResponse.AccountId,
                AccountName = $"account.{GenerateRandomString()}",
                Password = ValidAccountPassword,
                SuperPassword = ValidAccountPassword,
                Name = GenerateRandomString(),
                Phone = GenerateRandomString(),
                Email = GenerateRandomString(),
                Identity = GenerateRandomString(),
                Gender = Gender.Female
            };

            var convertResponse = await acClient.AccountConvertAsync(convertInfo);

            Assert.IsNotNull(convertResponse.AccountId);
            Assert.IsNotNull(convertResponse.AccountId, convertInfo.AccountId);
            Assert.AreEqual(convertResponse.AccountName, convertInfo.AccountName);
            Assert.AreEqual(convertResponse.Identity, convertInfo.Identity);
            Assert.AreEqual(convertResponse.Name, convertInfo.Name);
            Assert.AreEqual(convertResponse.Phone, convertInfo.Phone);
            Assert.AreEqual(convertResponse.Email, convertInfo.Email);
            Assert.AreEqual(convertResponse.Gender, convertInfo.Gender);
        }

        [TestMethod]
        public async Task E2E_AppClient_GuestAccess_DeviceInfoNull_Test()
        {
            await TestExpector.ExpectUCenterErrorAsync(
                UCenterErrorCode.DeviceInfoNull,
                async () => await acClient.AccountGuestAccessAsync(null));
        }

        [TestMethod]
        public async Task E2E_AppClient_GuestAccess_DeviceIdNull_Test()
        {
            await TestExpector.ExpectUCenterErrorAsync(
                UCenterErrorCode.DeviceIdNull,
                async () => await acClient.AccountGuestAccessAsync(new DeviceInfo()));
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

            await TestExpector.ExpectUCenterErrorAsync(
                UCenterErrorCode.AccountPasswordUnauthorized,
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
            Assert.AreEqual(registerResponse.Identity, uploadProfileResponse.Identity);
            Assert.AreEqual(registerResponse.Name, uploadProfileResponse.Name);
            Assert.AreEqual(registerResponse.Phone, uploadProfileResponse.Phone);
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
                Assert.AreEqual(registerResponse.Identity, uploadProfileResponse.Identity);
                Assert.AreEqual(registerResponse.Name, uploadProfileResponse.Name);
                Assert.AreEqual(registerResponse.Phone, uploadProfileResponse.Phone);
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