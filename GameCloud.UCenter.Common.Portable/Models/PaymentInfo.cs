using System.Runtime.Serialization;

namespace GameCloud.UCenter.Common.Models.PingPlusPlus
{
    [DataContract]
    public class PaymentInfo
    {
        public string AppId { get; set; }

        public string AccountId { get; set; }

        public string OrderNumber { get; set; }

        public string Channel { get; set; }

        public string Amount { get; set; }

        public string Currency { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}