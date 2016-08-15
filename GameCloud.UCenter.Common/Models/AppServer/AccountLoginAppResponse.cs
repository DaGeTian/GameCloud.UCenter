using System;
using System.Runtime.Serialization;

namespace GameCloud.UCenter.Common.Models.AppServer
{
    [DataContract]
    public class AccountLoginAppResponse
    {
        [DataMember]
        public string AccountId { get; set; }

        [DataMember]
        public string AccountName { get; set; }

        [DataMember]
        public string AccountToken { get; set; }

        [DataMember]
        public string AccountData { get; set; }

        [DataMember]
        public DateTime LastLoginDateTime { get; set; }
    }
}