using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GF.UCenter.MongoDB.Attributes;
using GF.UCenter.MongoDB.Entity;

namespace GF.Manager.Models
{
    [CollectionName("Plugin")]
    public class PluginEntity : EntityBase
    {
        public string Name { get; set; }

        public string Content { get; set; }
    }
}