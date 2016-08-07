using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace GF.Manager.Models
{
    [DataContract]
    public class PluginCategory : PluginBase
    {
        [DataMember(Name = "items")]
        public List<PluginItem> Items { get; set; }
    }
}