// Copyright(c) Cragon.All rights reserved.

namespace GameCloud.UCenter
{
    using System.Runtime.Serialization;

    [DataContract]
    public class AccountConvertInfo
    {
        [DataMember]
        public string AccountId { get; set; }

        [DataMember]
        public string AccountName { get; set; }

        [DataMember]
        public string OldPassword { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string SuperPassword { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string PhoneNum { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string IdentityNum { get; set; }

        [DataMember]
        public Gender Gender { get; set; }
    }
}