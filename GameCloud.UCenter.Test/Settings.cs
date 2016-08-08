using System.ComponentModel;
using System.ComponentModel.Composition;

namespace GameCloud.UCenter.Test
{
    [Export]
    public class Settings
    {
        [DefaultValue("localhost")]
        public string ServerHost { get; set; }

        [DefaultValue(8888)]
        public int ServerPort { get; set; }
    }
}