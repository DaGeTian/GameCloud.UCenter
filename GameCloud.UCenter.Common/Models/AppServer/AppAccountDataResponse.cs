using System.Runtime.Serialization;

namespace GameCloud.UCenter.Common.Models.AppServer
{
    [DataContract]
    public class AppAccountDataResponse
    {
        [DataMember]
        public string AccountId { get; set; }

        [DataMember]
        public string AppId { get; set; }

        [DataMember]
        public string Data { get; set; }
    }
}