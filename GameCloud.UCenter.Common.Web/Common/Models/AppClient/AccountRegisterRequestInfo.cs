using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using GameCloud.UCenter.Common.Portable.Models.AppClient;

namespace GameCloud.UCenter.Common.Models.AppClient
{
    [DataContract]
    public class AccountRegisterRequestInfo : AccountRegisterInfo
    {
        [DataMember]
        public override string AppId { get; set; }

        // todo: Set the valide rules of following properties.
        [DataMember]
        [Required]
        public override string AccountName { get; set; }

        [DataMember]
        [Required]
        public override string Password { get; set; }

        [DataMember]
        [Required]
        public override string SuperPassword { get; set; }

        [DataMember]
        public override string Name { get; set; }

        [DataMember]
        public override string Email { get; set; }
    }
}