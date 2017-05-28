using System.Runtime.Serialization;

namespace GameCloud.Manager.App.Models
{
    [DataContract]
    public class PluginCertificate
    {
        [DataMember]
        public string Thumbnail { get; set; }
    }
}
