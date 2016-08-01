using GF.Database;

namespace GF.UCenter.Test
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Common.Settings;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Web.Common;
    using Web.Common.Logger;

    [TestClass]
    public class UCenterTestBase
    {
        internal static ExportProvider ExportProvider;

        protected readonly TenantEnvironment Tenant;

        private static readonly List<IDisposable> DisposableList = new List<IDisposable>();
        private static readonly Lazy<List<char>> CharsPool = new Lazy<List<char>>(() =>
        {
            var chars = new List<char>();
            chars.AddRange(ParallelEnumerable.Range(48, 10).Select(i => (char)i)); // 0-9
            chars.AddRange(ParallelEnumerable.Range(65, 26).Select(i => (char)i)); // A-Z
            chars.AddRange(ParallelEnumerable.Range(97, 26).Select(i => (char)i)); // a-z
            return chars;
        }, LazyThreadSafetyMode.ExecutionAndPublication);

        public UCenterTestBase()
        {
            this.Tenant = ExportProvider.GetExportedValue<TenantEnvironment>();
            CustomTrace.Initialize(ExportProvider, "Trace.Console");
        }

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            AssemblyInitializeAsync().Wait();
        }

        [AssemblyCleanup]
        public static void AssemblyCleanUp()
        {
            foreach (var item in DisposableList)
            {
                if (item != null)
                {
                    item.Dispose();
                }
            }
        }

        protected string GenerateRandomString(int length = 8)
        {
            var random = new Random();
            var result = new List<char>();
            var maxIdx = CharsPool.Value.Count;
            result.Add(CharsPool.Value.ElementAt(random.Next(11, maxIdx)));

            for (var idx = 0; idx < length - 1; idx++)
            {
                result.Add(CharsPool.Value.ElementAt(random.Next(0, maxIdx)));
            }

            return string.Join(string.Empty, result);
        }

        private static async Task AssemblyInitializeAsync()
        {
            ExportProvider = CompositionContainerFactory.Create();

            SettingsInitializer.Initialize<Settings>(
                ExportProvider,
                SettingsDefaultValueProvider<Settings>.Default,
                AppConfigurationValueProvider.Default);

            SettingsInitializer.Initialize<Common.Settings.Settings>(
                ExportProvider,
                SettingsDefaultValueProvider<Common.Settings.Settings>.Default,
                AppConfigurationValueProvider.Default);

            SettingsInitializer.Initialize<DatabaseContextSettings>(
                ExportProvider,
                SettingsDefaultValueProvider<DatabaseContextSettings>.Default,
                AppConfigurationValueProvider.Default);

            var settings = ExportProvider.GetExportedValue<Common.Settings.Settings>();

            await InitProfileImageBlobsAsync(settings.DefaultProfileImageForFemaleBlobName);
            await InitProfileImageBlobsAsync(settings.DefaultProfileImageForMaleBlobName);
            await InitProfileImageBlobsAsync(settings.DefaultProfileThumbnailForFemaleBlobName);
            await InitProfileImageBlobsAsync(settings.DefaultProfileThumbnailForMaleBlobName);
        }

        private static async Task InitProfileImageBlobsAsync(string blobName)
        {
            using (var fileStream = File.OpenRead(@"TestData\github.png"))
            {
                var settings = ExportProvider.GetExportedValue<Common.Settings.Settings>();
                var blobContext = new StorageAccountContext(settings);
                await blobContext.UploadBlobAsync(blobName, fileStream, CancellationToken.None);
            }
        }
    }
}