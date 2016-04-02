﻿namespace GF.UCenter.Common.Portable.Models.AppClient
{
    using System.Runtime.Serialization;

    [DataContract]
    public class AccountResetPasswordInfo
    {
        [DataMember]
        public string AccountId { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string SuperPassword { get; set; }
    }
}