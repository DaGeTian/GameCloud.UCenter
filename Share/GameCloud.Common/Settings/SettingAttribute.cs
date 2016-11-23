using System;

namespace GameCloud.Common.Settings
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
