using System.Runtime.Serialization;

namespace GameCloud.UCenter.Common.Models.AppServer
{
    [DataContract]
    public class AppResponse
    {
        [DataMember]
        public string AppId { get; set; }

        [DataMember]
        public string AppSecret { get; set; }

        [DataMember]
        public string WechatAppId { get; set; }

        [DataMember]
        public string WechatAppSecret { get; set; }

        [DataMember]
        public string Token { get; set; }
    }
}