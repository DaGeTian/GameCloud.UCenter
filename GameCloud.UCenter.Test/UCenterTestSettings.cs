using System.ComponentModel;
using System.ComponentModel.Composition;

namespace GameCloud.UCenter.Test
{
    [Export]
    public class UCenterTestSettings
    {
        [DefaultValue("localhost")]
        public string UCenterServerHost { get; set; }

        [DefaultValue(8888)]
        public int UCenterServerPort { get; set; }
    }
}