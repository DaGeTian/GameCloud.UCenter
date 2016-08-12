using System.Runtime.Serialization;

namespace GameCloud.UCenter.Common.Models.PingPlusPlus
{
    [DataContract]
    public class ChargeInfo
    {
        [DataMember]
        public string AppId { get; set; }

        [DataMember]
        public string AppSecret { get; set; }

        [DataMember]
        public string AccountId { get; set; }

        [DataMember]
        public int Amount { get; set; }

        [DataMember]
        public string Subject { get; set; }

        [DataMember]
        public string Body { get; set; }

        [DataMember]
        public string ClientIp { get; set; }

        [DataMember]
        public string Description { get; set; }
    }
}