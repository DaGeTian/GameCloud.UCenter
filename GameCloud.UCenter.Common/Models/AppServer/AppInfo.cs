using System.Runtime.Serialization;
using GameCloud.UCenter.Common.Dumper;

namespace GameCloud.UCenter.Common.Models.AppServer
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