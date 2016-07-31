using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using GF.UCenter.MongoDB.Attributes;

namespace GF.Manager.Models
{
    [CollectionName("PluginItem")]
    public class PluginItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public HttpMethod Method { get; set; }

        public IReadOnlyList<PluginItem> Items { get; set; }
    }
}