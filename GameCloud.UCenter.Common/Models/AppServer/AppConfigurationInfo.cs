using System.Runtime.Serialization;

namespace GameCloud.UCenter.Common.Models.AppServer
{
    [DataContract]
    public class AppConfigurationInfo
    {
        [DataMember]
        public virtual string AppId { get; set; }

        [DataMember]
        public virtual string Configuration { get; set; }
    }
}