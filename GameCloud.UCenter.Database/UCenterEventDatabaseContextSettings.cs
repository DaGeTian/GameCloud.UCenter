using System.ComponentModel.Composition;
using GameCloud.Database;
using GameCloud.UCenter.Common.Settings;

namespace GameCloud.UCenter.Database
{
    [Export]
    public class UCenterEventDatabaseContextSettings : DatabaseContextSettings
    {
        [Setting(settingName: "EventConnectionString")]
        public override string ConnectionString { get; set; }

        [Setting(settingName: "EventDatabaseName")]
        public override string DatabaseName { get; set; }
    }
}
