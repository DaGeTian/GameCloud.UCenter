using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GF.Database;
using GF.Manager.Models;
using GF.UCenter.Common;
using GF.UCenter.Common.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace GF.Manager.Tests
{
    [TestClass]
    public class ManagerTest
    {
        private static ExportProvider ExportProvider;

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            ExportProvider = CompositionContainerFactory.Create();

            SettingsInitializer.Initialize<Settings>(
                ExportProvider,
                SettingsDefaultValueProvider<Settings>.Default,
                AppConfigurationValueProvider.Default);

            SettingsInitializer.Initialize<UCenter.Common.Settings.Settings>(
                ExportProvider,
                SettingsDefaultValueProvider<UCenter.Common.Settings.Settings>.Default,
                AppConfigurationValueProvider.Default);

            SettingsInitializer.Initialize<DatabaseContextSettings>(
                ExportProvider,
                SettingsDefaultValueProvider<DatabaseContextSettings>.Default,
                AppConfigurationValueProvider.Default);
        }

        [TestClass]
        public void InitData()
        {
            var plugin = JsonConvert.DeserializeObject<Plugin>;
            Plugin plugin = new Plugin()
            {
                Name = "UCenter",
                Description = "UCenter management",
                ServerUrl = "http://abc"
            };


        }
    }
}
