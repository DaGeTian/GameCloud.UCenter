using System.Runtime.Serialization;


namespace GameCloud.UCenter.Common.Portable.Models.AppClient
{
    public class AccountWeChatOAuthInfo
    {
        [DataMember]
        public string AccountId { get; set; }

        [DataMember]
        public string AppId { get; set; }

        [DataMember]
        public string Code { get; set; }
    }
}
