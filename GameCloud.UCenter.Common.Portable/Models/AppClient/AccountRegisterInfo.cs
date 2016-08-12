using System.Runtime.Serialization;

namespace GameCloud.UCenter.Common.Portable.Models.AppClient
{
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

        [DataMember]
        public DeviceInfo Device { get; set; }
    }
}