﻿namespace GF.UCenter.Common
{
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;
    using Portable.Models.AppClient;

    [DataContract]
    public class AccountRegisterRequestInfo : AccountRegisterInfo
    {
        // todo: Set the valide rules of following properties.
        [DataMember]
        //// [RegularExpression(UCenterModelRules.AccountNameRule)]
        [Required]
        public override string AccountName { get; set; }

        [DataMember]
        //// [RegularExpression(UCenterModelRules.PasswordRule)]
        [Required]
        [DumperTo("<--Passowrd-->")]
        public override string Password { get; set; }

        [DataMember]
        //// [RegularExpression(UCenterModelRules.PasswordRule)]
        [Required]
        [DumperTo("<--SuperPassword-->")]
        public override string SuperPassword { get; set; }

        [DataMember]
        //// [StringLength(32, MinimumLength = 4)]
        public override string Name { get; set; }

        [DataMember]
        //// [EmailAddress]
        public override string Email { get; set; }
    }
}