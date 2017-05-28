using Newtonsoft.Json;
using System.Collections.Generic;

namespace GameCloud.UCenter.Common.Portable.Models.AppClient
{
    public class Charge
    {
        [JsonProperty("amount")]
        public int? Amount { get; set; }
        [JsonProperty("amount_refunded")]
        public int? AmountRefunded { get; set; }
        [JsonProperty("amount_settle")]
        public int? AmountSettle { get; set; }
        [JsonProperty("app")]
        public string App { get; set; }
        [JsonProperty("body")]
        public string Body { get; set; }
        [JsonProperty("channel")]
        public string Channel { get; set; }
        [JsonProperty("client_ip")]
        public string ClientIp { get; set; }
        [JsonProperty("created")]
        public int? Created { get; set; }
        [JsonProperty("credential")]
        public object Credential { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("extra")]
        public Dictionary<string, object> Extra { get; set; }
        [JsonProperty("failure_code")]
        public string FailureCode { get; set; }
        [JsonProperty("failure_msg")]
        public string FailureMsg { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("livemode")]
        public bool Livemode { get; set; }
        [JsonProperty("metadata")]
        public Dictionary<string, object> Metadata { get; set; }
        [JsonProperty("object")]
        public string Object { get; set; }
        [JsonProperty("order_no")]
        public string OrderNo { get; set; }
        [JsonProperty("paid")]
        public bool Paid { get; set; }
        [JsonProperty("refunded")]
        public bool Refunded { get; set; }
        //[JsonProperty("refunds")]
        //public RefundList Refunds { get; set; }
        [JsonProperty("subject")]
        public string Subject { get; set; }
        [JsonProperty("time_expire")]
        public int? TimeExpire { get; set; }
        [JsonProperty("time_paid")]
        public int? TimePaid { get; set; }
        [JsonProperty("time_settle")]
        public int? TimeSettle { get; set; }
        [JsonProperty("transaction_no")]
        public string TransactionNo { get; set; }
    }
}
