using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace GF.Manager.Models
{
    public class PluginRequest
    {
        public string Url { get; set; }

        public string Method { get; set; }

        public string Content { get; set; }
    }
}