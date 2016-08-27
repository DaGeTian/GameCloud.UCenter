using System.ComponentModel;
using System.ComponentModel.Composition;

namespace GameCloud.UCenter.Test
{
    [Export]
    public class UCenterTestSettings
    {
        [DefaultValue("10.0.0.4")]
        public string UCenterServerHost { get; set; }

        [DefaultValue(80)]
        public int UCenterServerPort { get; set; }
    }
}