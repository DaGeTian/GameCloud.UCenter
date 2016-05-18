using System;

namespace GF.UCenter.CouchBase
{
    [DocumentType("Order")]
    public class OrderEntity : BaseEntity<OrderEntity>
    {
        public string AppId { get; set; }

        public string AccountId { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public string RawData { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime CompletedTime { get; set; }
    }
}