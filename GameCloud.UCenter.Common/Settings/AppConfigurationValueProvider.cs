using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web.Configuration;
using System.Web.Hosting;

namespace GameCloud.UCenter.Common.Settings
{
    public class AppConfigurationValueProvider : ISettingsValueProvider
    {
        private static readonly Lazy<AppConfigurationValueProvider> DefaultProvider = new Lazy<AppConfigurationValueProvider>(
            () =>
            {
                Configuration configuration = null;
                configuration = HostingEnvironment.IsHosted
                    ? WebConfigurationManager.OpenWebConfiguration("~/")
                    : ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                return new AppConfigurationValueProvider(configuration);
            },
            LazyThreadSafetyMode.PublicationOnly);

        private readonly Configuration configuration;
        private readonly List<SettingsValuePair> settingValues = new List<SettingsValuePair>();

        public AppConfigurationValueProvider(Configuration configuration)
        {
            this.configuration = configuration;
            var appSettings = this.configuration.AppSettings.Settings;
            var settings = appSettings.OfType<KeyValueConfigurationElement>().ToDictionary(
                element => element.Key,
                element => element.Value,
                StringComparer.Ordinal);

            foreach (var kv in settings)
            {
                this.settingValues.Add(new SettingsValuePair { Name = kv.Key, Value = kv.Value });
            }
        }

        public static AppConfigurationValueProvider Default
        {
            get { return DefaultProvider.Value; }
        }

        public ICollection<SettingsValuePair> SettingValues
        {
            get { return this.settingValues; }
        }
    }
}