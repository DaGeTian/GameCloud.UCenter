using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using GF.Manager.Contract;

namespace GF.Manager.Models
{
    [DataContract]
    public class Plugin : PluginBase
    {
        public string ServerUrl { get; set; }

        [DataMember(Name = "categories")]
        public List<PluginCategory> Categories { get; set; }

        [DataMember(Name = "items")]
        public List<PluginItem> Items { get; set; }

        public PluginEntryPoint EntryPoint { get; set; }

        public PluginItem GetItem(string itemName)
        {
            foreach (var cat in this.Categories)
            {
                if (cat.Items.Any(i => i.Name == itemName))
                {
                    return cat.Items.First(i => i.Name == itemName);
                }
            }

            return this.Items.FirstOrDefault(i => i.Name == itemName);
        }

        [DataMember(Name = "scripts")]
        public List<string> Scripts { get; set; }

        [DataMember(Name = "styles")]
        public List<string> Styles { get; set; }
    }
}