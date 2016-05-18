using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GF.UCenter.CouchBase;

namespace GF.UCenter.Web.Models
{
    public class ListModel<TRaw>
    {
        public int Total { get; set; }

        public int Page { get; set; }

        public int Count { get; set; }

        public IEnumerable<TRaw> Raws { get; set; }
    }

    public class OrderRaw
    {
        public string AccountId { get; set; }

        public string AccountName { get; set; }

        public string OrderId { get; set; }

        public string AppId { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public string OrderData { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime FinishTime { get; set; }
    }
}