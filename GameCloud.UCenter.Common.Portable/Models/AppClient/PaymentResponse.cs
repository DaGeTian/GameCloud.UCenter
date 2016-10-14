using System;
using System.Runtime.Serialization;

namespace GameCloud.UCenter.Common.Portable.Models.AppClient
{
    [DataContract]
    public class PaymentResponse
    {
        [DataMember]
        public string AccountId { get; set; }

        [DataMember]
        public string AccountName { get; set; }

        [DataMember]
        public Charge Charge { get; set; }
    }
}