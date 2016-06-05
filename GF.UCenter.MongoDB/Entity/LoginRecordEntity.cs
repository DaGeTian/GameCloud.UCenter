namespace GF.UCenter.MongoDB.Entity
{
    using System;
    using Attributes;
    using Common.Portable.Contracts;

    [CollectionName("LoginRecord")]
    public class LoginRecordEntity : EntityBase
    {
        public string AccountName { get; set; }

        public string AccountId { get; set; }

        public DateTime LoginTime { get; set; }

        public string ClientIp { get; set; }

        public string UserAgent { get; set; }

        public UCenterErrorCode Code { get; set; }

        public string LoginArea { get; set; }

        public string Comments { get; set; }
    }
}
