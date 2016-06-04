namespace GF.UCenter.MongoDB.Entity
{
    using System;
    using Attributes;

    [CollectionName("Order")]
    public class OrderEntity : EntityBase
    {
        public string AppId { get; set; }

        public string AppName { get; set; }

        public string AccountId { get; set; }

        public string AccountName { get; set; }

        public OrderState State { get; set; }

        public string Content { get; set; }

        public DateTime CompletedTime { get; set; }
    }
}
