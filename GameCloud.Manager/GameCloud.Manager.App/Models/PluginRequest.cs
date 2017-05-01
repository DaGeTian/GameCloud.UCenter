using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using GameCloud.Manager.PluginContract.Requests;

namespace GameCloud.Manager.App.Models
{
    [DataContract]
    public class PluginRequest
    {
        [DataMember(Name = "pluginName")]
        public string PluginName { get; set; }

        [DataMember(Name = "categoryName")]
        public string CategoryName { get; set; }

        [DataMember(Name = "itemName")]
        public string ItemName { get; set; }

        [DataMember(Name = "method")]
        public PluginRequestMethod Method { get; set; }

        [DataMember(Name = "content")]
        public Dictionary<string, string> Content { get; set; }
    }
}
