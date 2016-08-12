// Copyright(c) Cragon.All rights reserved.

namespace GameCloud.UCenter
{
    using System.Runtime.Serialization;

    [DataContract]
    public class AccountResetPasswordInfo
    {
        [DataMember]
        public string AccountName { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string SuperPassword { get; set; }
    }
}