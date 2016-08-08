using System.Runtime.Serialization;

namespace GameCloud.UCenter.Common.Portable.Models.Ip
{
    [DataContract]
    public class IPInfoResponse
    {
        [DataMember(Name = "code")]
        public IPInfoResponseCode Code { get; set; }

        [DataMember(Name = "data")]
        public IPInfoResponseContent Content { get; set; }
    }
}
