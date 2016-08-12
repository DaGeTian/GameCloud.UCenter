using System.Collections.Generic;

namespace GameCloud.UCenter
{
    public interface ISettingsValueProvider
    {
        ICollection<SettingsValuePair> SettingValues { get; }
    }
}