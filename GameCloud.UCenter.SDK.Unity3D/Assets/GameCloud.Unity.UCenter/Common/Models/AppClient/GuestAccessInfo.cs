using System.Runtime.Serialization;

namespace GameCloud.UCenter.Common.Portable.Models.AppClient
{
    [DataContract]
    public class GuestAccessInfo
    {
        [DataMember]
        public string AppId { get; set; }

        [DataMember]
        public DeviceInfo Device { get; set; }
    }
}