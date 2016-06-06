namespace GF.UCenter.MongoDB.Entity
{
    using System;
    using Attributes;

    /// <summary>
    /// Provide an order entity.
    /// </summary>
    [CollectionName("Order")]
    public class OrderEntity : EntityBase
    {
        /// <summary>
        /// Gets or sets the application id.
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// Gets or sets the application name.
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// Gets or sets the account id.
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Gets or sets the account name.
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// Gets or sets the order state.
        /// </summary>
        public OrderState State { get; set; }

        /// <summary>
        /// Gets or sets the order content.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the order created time.
        /// </summary>
        public DateTime CompletedTime { get; set; }
    }
}
