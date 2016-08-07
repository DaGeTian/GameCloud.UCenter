using System;
using System.Collections.Generic;
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
        private readonly string configFile;

        public PluginConfiguration(string configFile)
        {
            this.configFile = configFile;
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
            var settings = new List<PluginSetting>();
            if (File.Exists(this.configFile))
            {
                XDocument doc = XDocument.Load(this.configFile);
                doc.XPathSelectElements("configuration/settings/setting")
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

            this.Settings = settings;
        }
    }
}
