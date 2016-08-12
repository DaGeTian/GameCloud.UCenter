using System.Runtime.Serialization;

namespace GameCloud.UCenter
{
    [DataContract]
    public class AppResponse
    {
        [DataMember]
        public string AppId { get; set; }

        [DataMember]
        public string AppSecret { get; set; }

        [DataMember]
        public string Token { get; set; }
    }
}