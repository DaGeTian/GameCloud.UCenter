using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace GF.Manager.Contract.Configuration
{
    public class PluginConfiguration
    {
        private readonly string defaultConfigFile;
        private readonly string gloablConfigFile;
        private readonly string pluginName;

        public PluginConfiguration(string pluginName)
        {
            string rootPath = Path.Combine(HttpRuntime.AppDomainAppPath, "plugins");
            this.defaultConfigFile = Path.Combine(rootPath, pluginName, "config.xml");
            this.gloablConfigFile = Path.Combine(rootPath, "global-config.xml");
            this.pluginName = pluginName;
            this.Init();
        }

        public IReadOnlyList<PluginSetting> Settings { get; set; }

        public PluginSetting GetSetting(string key)
        {
            return this.Settings.Where(s => s.Key == key).FirstOrDefault();
        }

        public T GetSettingValue<T>(string key)
        {
            var setting = this.GetSetting(key);
            if (setting == null)
            {
                return default(T);
            }

            return (T)Convert.ChangeType(setting.Value, typeof(T));
        }

        private void Init()
        {
            var defaultSettings = this.GetSettings(this.defaultConfigFile, @"configuration/settings/setting");
            var globalSettings = this.GetSettings(this.gloablConfigFile, string.Format(CultureInfo.InvariantCulture, @"plugins/plugin[@name='{0}']/settings/setting", this.pluginName));

            var settings = globalSettings
                .Concat(defaultSettings.Where(s => !globalSettings.Any(gs => gs.Key == s.Key)))
                .ToList();

            this.Settings = settings;
        }

        private IReadOnlyList<PluginSetting> GetSettings(string filePath, string xpath)
        {
            var settings = new List<PluginSetting>();
            if (File.Exists(filePath))
            {
                XDocument doc = XDocument.Load(filePath);
                doc.XPathSelectElements(xpath)
                    .ToList()
                    .ForEach(e =>
                    {
                        var setting = new PluginSetting()
                        {
                            Key = e.Attribute("key").Value,
                            Value = e.Attribute("value").Value
                        };

                        settings.Add(setting);
                    });
            }

            return settings;
        }
    }
}
