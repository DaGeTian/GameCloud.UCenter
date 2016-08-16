using System.Runtime.Serialization;

namespace GameCloud.UCenter.Common.Portable.Models.AppClient
{
    [DataContract]
    public class GuestAccessResponse
    {
        [DataMember]
        public string AccountId { get; set; }

        [DataMember]
        public string AccountName { get; set; }

        [DataMember]
        public AccountType AccountType { get; set; }

        [DataMember]
        public string Token { get; set; }

        [DataMember]
        public string Password { get; set; }
    }
}