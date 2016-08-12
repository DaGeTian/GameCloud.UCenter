// Copyright(c) Cragon.All rights reserved.

namespace GameCloud.UCenter
{
    using System.Runtime.Serialization;

    [DataContract]
    public class AccountLoginInfo
    {
        [DataMember]
        public string AccountName { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public DeviceInfo Device { get; set; }
    }
}