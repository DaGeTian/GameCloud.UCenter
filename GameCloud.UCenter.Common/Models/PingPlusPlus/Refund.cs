﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameCloud.UCenter
{
    public class Refund
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("order_no")]
        public string OrderNo { get; set; }

        [JsonProperty("amount")]
        public int? Amount { get; set; }

        [JsonProperty("succeed")]
        public bool Succeed { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("created")]
        public int? Created { get; set; }

        [JsonProperty("time_succeed")]
        public int? TimeSucceed { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("failure_code")]
        public string FailureCode { get; set; }

        [JsonProperty("failure_msg")]
        public string FailureMsg { get; set; }

        [JsonProperty("metadata")]
        public Dictionary<string, object> Metadata { get; set; }

        [JsonProperty("charge")]
        public string Charge { get; set; }
    }
}