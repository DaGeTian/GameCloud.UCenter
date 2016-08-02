using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace GF.Manager.Models
{
    [DataContract]
    public class Plugin
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        public string ServerUrl { get; set; }

        public IReadOnlyList<PluginItem> Items { get; set; }
    }
}