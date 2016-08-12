// Copyright(c) Cragon.All rights reserved.

namespace GameCloud.UCenter
{
    using GameCloud.Database;
    using GameCloud.Database.Attributes;

    /// <summary>
    /// Provide a class for key placeholder.
    /// </summary>
    [CollectionName("KeyPlaceholder")]
    public class KeyPlaceholderEntity : EntityBase
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public KeyType Type { get; set; }

        /// <summary>
        /// Gets or sets the account id.
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Gets or sets the account name.
        /// </summary>
        public string AccountName { get; set; }
    }
}
