// Copyright(c) Cragon.All rights reserved.

namespace GameCloud.UCenter
{
    using System;
    using GameCloud.Database;
    using GameCloud.Database.Attributes;

    /// <summary>
    /// Provide an account event entity class.
    /// </summary>
    [CollectionName("AccountEvent")]
    public class AccountEventEntity : EntityBase
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
        /// Gets or sets the account Id.
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// Gets or sets the client IP.
        /// </summary>
        public string ClientIp { get; set; }

        /// <summary>
        /// Gets or sets the login area.
        /// </summary>
        public string LoginArea { get; set; }

        /// <summary>
        /// Gets or sets the user agent.
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        public string Message { get; set; }
    }
}
