using System.Runtime.Serialization;
using GameCloud.UCenter.Dumper;

namespace GameCloud.UCenter
{
    [DataContract]
    public class AccountLoginAppInfo
    {
        [DataMember]
        public virtual string AppId { get; set; }

        [DataMember]
        [DumperTo("<--AppSecret-->")]
        public virtual string AppSecret { get; set; }

        [DataMember]
        public virtual string AccountId { get; set; }

        [DataMember]
        [DumperTo("<--AppSecret-->")]
        public virtual string AccountToken { get; set; }
    }
}