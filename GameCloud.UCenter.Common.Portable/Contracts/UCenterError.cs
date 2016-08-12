// Copyright(c) Cragon.All rights reserved.

namespace GameCloud.UCenter
{
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    [DataContract]
    public class UCenterError
    {
        [DataMember]
        [JsonProperty("code")]
        public UCenterErrorCode ErrorCode { get; set; }

        [DataMember]
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}