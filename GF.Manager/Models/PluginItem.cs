using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization;
using GF.Manager.Contract;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GF.Manager.Models
{
    [DataContract]
    public class PluginItem : PluginBase
    {
        private string controller;

        public string Url { get; set; }

        [DataMember(Name = "type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public PluginItemType Type { get; set; }

        [DataMember(Name = "method")]
        public HttpMethod Method { get; set; }

        [DataMember(Name = "view")]
        public string View { get; set; }

        [DataMember(Name = "controller")]
        public string Controller
        {
            get
            {
                if (string.IsNullOrEmpty(this.controller))
                {
                    return this.Type.ToString().ToLowerInvariant() + "Controller";
                }

                return this.controller;
            }
            set
            {
                this.controller = value;
            }
        }

        public MethodInfo EntryMethod { get; set; }
    }
}
