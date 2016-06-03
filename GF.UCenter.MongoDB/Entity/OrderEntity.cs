using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GF.UCenter.MongoDB.Attributes;

namespace GF.UCenter.MongoDB.Entity
{
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
