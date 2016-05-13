namespace GF.UCenter.Common.Portable
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