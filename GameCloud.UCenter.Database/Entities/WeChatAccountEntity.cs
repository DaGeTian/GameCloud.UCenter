using GameCloud.Database;
using GameCloud.Database.Attributes;

namespace GameCloud.UCenter.Database.Entities
{
    /// <summary>
    /// Provide a class for account document.
    /// </summary>
    [CollectionName("WeChatAccount")]
    public class WeChatAccountEntity : EntityBase
    {
        /// <summary>
        /// Gets or sets the account name.
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Gets or sets the account name.
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// Gets or sets open id
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// Gets or sets union id
        /// </summary>
        public string UnionId { get; set; }
    }
}