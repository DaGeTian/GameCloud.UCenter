using GF.MongoDB.Common;
using GF.MongoDB.Common.Attributes;

namespace GF.UCenter.Entity
{
    /// <summary>
    /// Provide a class for application account data.
    /// </summary>
    [CollectionName("AppAccountData")]
    public class AppAccountDataEntity : EntityBase
    {
        /// <summary>
        /// Gets or sets the application id.
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// Gets or sets the account id.
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        public string Data { get; set; }
    }
}