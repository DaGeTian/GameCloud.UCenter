namespace GF.UCenter.Common
{
    using System.Runtime.Serialization;

    [DataContract]
    public class AppConfigurationInfo
    {
        [DataMember]
        public virtual string AppId { get; set; }

        [DataMember]
        public virtual string Configuration { get; set; }
    }
}