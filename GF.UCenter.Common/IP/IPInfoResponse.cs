namespace GF.UCenter.Common.IP
{
    using System.Runtime.Serialization;

    [DataContract]
    public class IPInfoResponse
    {
        [DataMember(Name = "code")]
        public IPInfoResponseCode Code { get; set; }

        [DataMember(Name = "data")]
        public IPInfo Info { get; set; }
    }
}
