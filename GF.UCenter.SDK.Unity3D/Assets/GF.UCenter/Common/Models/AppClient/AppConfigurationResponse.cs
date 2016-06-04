using System.Runtime.Serialization;

namespace GF.UCenter.Common.Portable.Models.AppClient
{
    [DataContract]
    public class AppConfigurationResponse
    {
        [DataMember]
        public string AppId { get; set; }

        [DataMember]
        public string Configuration { get; set; }
    }
}