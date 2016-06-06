namespace GF.UCenter.MongoDB.Entity
{
    using System;
    using Attributes;
    using Common.Portable.Contracts;

    /// <summary>
    /// Provide a login record class.
    /// </summary>
    [CollectionName("LoginRecord")]
    public class LoginRecordEntity : EntityBase
    {
        /// <summary>
        /// Gets or sets the account name.
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// Gets or sets the account Id.
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Gets or sets the login time.
        /// </summary>
        public DateTime LoginTime { get; set; }

        /// <summary>
        /// Gets or sets the client IP.
        /// </summary>
        public string ClientIp { get; set; }

        /// <summary>
        /// Gets or sets the user agent.
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        public UCenterErrorCode Code { get; set; }

        /// <summary>
        /// Gets or sets the login area.
        /// </summary>
        public string LoginArea { get; set; }

        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        public string Comments { get; set; }
    }
}
