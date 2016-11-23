using System.Collections.Generic;

namespace GameCloud.Common.Settings
{
    public interface ISettingsValueProvider
    {
        ICollection<SettingsValuePair> SettingValues { get; }
    }
}