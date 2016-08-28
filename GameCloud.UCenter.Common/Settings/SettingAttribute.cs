using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCloud.UCenter.Common.Settings
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class SettingAttribute : Attribute
    {
        public SettingAttribute(string settingName)
        {
            this.SettingName = settingName;
        }

        public string SettingName { get; private set; }
    }
}
