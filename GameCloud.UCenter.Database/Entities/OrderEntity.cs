using GameCloud.Database;
using GameCloud.Database.Attributes;
using System.Collections.Generic;
using GameCloud.UCenter.Common.Portable.Models.AppClient;

namespace GameCloud.UCenter.Database.Entities
{
    [CollectionName("Order")]
    public class OrderEntity : EntityBase
    {
        public string Amount { get; set; }

        public string Channel { get; set; }

        public string Currency { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string ClientIp { get; set; }

        public OrderStatus Status { get; set; }
    }
}
