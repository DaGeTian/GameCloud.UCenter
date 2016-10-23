using System;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;

namespace GameCloud.UCenter.Common.Settings
{
    public static class SettingsInitializer
    {
        public static TSettings Initialize<TSettings>(ExportProvider exportProvider,
            params ISettingsValueProvider[] providers)
        {
            var settings = exportProvider.GetExportedValue<TSettings>();
            var properties = typeof(TSettings).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var provider in providers)
            {
                properties.AsParallel()
                    .ForAll(prop =>
                    {
                        string name = null;
                        var attr = prop.GetCustomAttribute<SettingAttribute>();
                        if (attr != null)
                        {
                            name = attr.SettingName;
                        }
                        else
                        {
                            name = prop.Name;
                        }

                        var pair = provider.SettingValues.FirstOrDefault(s => s.Name == name);
                        if (pair != null)
                        {
                            prop.SetValue(settings, Convert.ChangeType(pair.Value, prop.PropertyType));
                        }
                    });
            }

            return settings;
        }
    }
}