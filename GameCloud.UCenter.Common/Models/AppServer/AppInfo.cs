using System.Runtime.Serialization;
using GameCloud.UCenter.Dumper;

namespace GameCloud.UCenter
{
    [DataContract]
    public class AppInfo
    {
        [DataMember]
        public virtual string AppId { get; set; }

        [DataMember]
        [DumperTo("<--AppSecret-->")]
        public virtual string AppSecret { get; set; }
    }
}