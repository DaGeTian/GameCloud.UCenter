using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GF.UCenter.CouchBase;

namespace GF.UCenter.Web.Common.Modes
{
    public class OrderRaw
    {
        public string AccountId { get; set; }

        public string AccountName { get; set; }

        public string OrderId { get; set; }

        public string AppId { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public string RawData { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime CompletedTime { get; set; }
    }
}
