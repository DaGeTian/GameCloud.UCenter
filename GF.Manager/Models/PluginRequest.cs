using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Web;
using GF.Manager.Contract;

namespace GF.Manager.Models
{
    [DataContract]
    public class PluginRequest
    {
        [DataMember(Name = "pluginName")]
        public string PluginName { get; set; }

        [DataMember(Name = "category")]
        public string Category { get; set; }

        [DataMember(Name = "item")]
        public string Item { get; set; }

        [DataMember(Name = "method")]
        public PluginRequestMethod Method { get; set; }

        [DataMember(Name = "content")]
        public Dictionary<string, string> Content { get; set; }
    }
}