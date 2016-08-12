// Copyright(c) Cragon.All rights reserved.

namespace GameCloud.UCenter
{
    using System.Runtime.Serialization;

    [DataContract]
    public class AccountRegisterInfo
    {
        [DataMember]
        public virtual string AccountName { get; set; }

        [DataMember]
        public virtual string Password { get; set; }

        [DataMember]
        public virtual string SuperPassword { get; set; }

        [DataMember]
        public virtual string Name { get; set; }

        [DataMember]
        public virtual string PhoneNum { get; set; }

        [DataMember]
        public virtual string Email { get; set; }

        [DataMember]
        public string IdentityNum { get; set; }

        [DataMember]
        public Gender Gender { get; set; }
    }
}