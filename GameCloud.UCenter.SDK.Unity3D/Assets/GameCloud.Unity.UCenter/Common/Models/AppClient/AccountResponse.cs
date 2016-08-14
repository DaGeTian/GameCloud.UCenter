using System;
using System.Runtime.Serialization;

namespace GameCloud.UCenter.Common.Portable.Models.AppClient
{
    [DataContract]
    public class AccountResponse
    {
        [DataMember]
        public string AccountId { get; set; }

        [DataMember]
        public string AccountName { get; set; }

        [DataMember]
        public AccountStatus AccountStatus { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string SuperPassword { get; set; }

        [DataMember]
        public string Token { get; set; }

        [DataMember]
        public DateTime LastLoginDateTime { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string ProfileImage { get; set; }

        [DataMember]
        public string ProfileThumbnail { get; set; }

        [DataMember]
        public Gender Gender { get; set; }

        [DataMember]
        public string Identity { get; set; }

        [DataMember]
        public string Phone { get; set; }

        [DataMember]
        public string Email { get; set; }
    }
}