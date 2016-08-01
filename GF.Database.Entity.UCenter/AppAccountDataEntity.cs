using GF.Database.Entity.Common;
using GF.Database.Entity.Common.Attributes;

namespace GF.Database.Entity.UCenter
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