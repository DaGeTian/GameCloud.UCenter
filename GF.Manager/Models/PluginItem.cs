using System;
using System.Collections.Generic;
using System.Net.Http;

namespace GF.Manager.Models
{
    public class PluginItem
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }
        
        public string Url { get; set; }

        public HttpMethod Method { get; set; }

        public IReadOnlyList<PluginItem> Items { get; set; }
    }
}