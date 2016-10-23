using System.Runtime.Serialization;

namespace GameCloud.UCenter.Common.Portable.Models.AppClient
{
    [DataContract]
    public class AccountRequestResponse
    {
        [DataMember]
        public string AccountId { get; set; }

        [DataMember]
        public string AccountName { get; set; }

        [DataMember]
        public AccountType AccountType { get; set; }

        [DataMember]
        public AccountStatus AccountStatus { get; set; }

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

        public virtual void ApplyEntity(AccountResponse account)
        {
            this.AccountId = account.AccountId;
            this.AccountName = account.AccountName;
            this.AccountStatus = account.AccountStatus;
            this.Name = account.Name;
            this.ProfileImage = account.ProfileImage;
            this.ProfileThumbnail = account.ProfileThumbnail;
            this.Gender = account.Gender;
            this.Identity = account.Identity;
            this.Phone = account.Phone;
            this.Email = account.Email;
        }
    }
}