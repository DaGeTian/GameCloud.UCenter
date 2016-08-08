using System.Runtime.Serialization;

namespace GameCloud.UCenter.Common.Portable.Models.AppClient
{
    [DataContract]
    public class AccountResetPasswordResponse : AccountRequestResponse
    {
        public override void ApplyEntity(AccountResponse account)
        {
            base.ApplyEntity(account);
        }
    }
}