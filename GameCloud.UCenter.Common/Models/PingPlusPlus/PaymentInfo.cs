using System.Runtime.Serialization;

namespace GameCloud.UCenter.Common.Models.PingPlusPlus
{
    [DataContract]
    public class PaymentInfo
    {
        [DataMember(Name = "order_no")]
        public string OrderNo { get; set; }

        [DataMember(Name = "channel")]
        public string Channel { get; set; }

        [DataMember(Name = "amount")]
        public string Amount { get; set; }

        [DataMember(Name = "currency")]
        public string Currency { get; set; }

        [DataMember(Name = "subject")]
        public string Subject { get; set; }

        [DataMember(Name = "body")]
        public string Body { get; set; }
    }
}