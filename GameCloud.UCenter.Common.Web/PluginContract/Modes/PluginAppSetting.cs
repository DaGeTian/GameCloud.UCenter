using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameCloud.Manager.PluginContract.Models
{
    public class PluginAppSetting
    {
        public PluginAppSetting(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public string Key { get; private set; }

        public string Value { get; private set; }
    }
}
