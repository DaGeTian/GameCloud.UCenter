using System.Runtime.Serialization;

namespace GameCloud.UCenter.Common.Models.AppServer
{
    [DataContract]
    public class AppAccountDataInfo
    {
        [DataMember]
        public virtual string AccountId { get; set; }

        [DataMember]
        public virtual string AppId { get; set; }

        [DataMember]
        public virtual string AppSecret { get; set; }

        [DataMember]
        public virtual string Data { get; set; }
    }
}