namespace GF.UCenter.Web.Common.Modes
{
    using System;
    using MongoDB.Entity;

    public class OrderRaw
    {
        public string AccountId { get; set; }

        public string AccountName { get; set; }

        public string OrderId { get; set; }

        public string AppId { get; set; }

        public OrderState State { get; set; }

        public string Content { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime CompletedTime { get; set; }
    }
}
