using System;
using System.Runtime.Serialization;

namespace GameCloud.UCenter.Common.Portable.Models.AppClient
{
    [DataContract]
    public class AccountLoginResponse : AccountRequestResponse
    {
        [DataMember]
        public string Token { get; set; }

        [DataMember]
        public DateTime LastLoginDateTime { get; set; }

        public override void ApplyEntity(AccountResponse account)
        {
            this.Token = account.Token;
            this.LastLoginDateTime = account.LastLoginDateTime;
            base.ApplyEntity(account);
        }
    }
}