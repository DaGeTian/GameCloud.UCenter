using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GF.Manager.Contract;

namespace GF.Manager.PluginDemo
{
    [PluginMetadata(Name = "demopluin", DisplayName = "Demo plugin", Description = "This is a demo plugin.")]
    [PluginCollectionMetadata(Name = "coll1", DisplayName = "Collection 1", Description = "This is demo collection 1")]
    [PluginCollectionMetadata(Name = "coll2", DisplayName = "Collection 2", Description = "This is demo collection 2")]
    [PluginCollectionMetadata(Name = "coll3", DisplayName = "Collection 3", Description = "This is demo collection 3")]
    public class DemoPlugin : PluginEntryPoint
    {
        [PluginItemMetadata(Name = "data-search", Collection = "coll1", DisplayName = "Demo data manager", Description = "This is a demo data list manager", Type = PluginItemType.Search, View = "demo-search.html")]
        public List<DemoPluginRawData> GetData(PluginRequestInfo<string> requst)
        {
            return new List<DemoPluginRawData>()
            {
                new DemoPluginRawData() { Id=1, Name="data 1" },
                new DemoPluginRawData() { Id=2, Name="data 2" },
                new DemoPluginRawData() { Id=3, Name="data 3" },
                new DemoPluginRawData() { Id=4, Name="data 4" },
                new DemoPluginRawData() { Id=5, Name="data 5" }
            };
        }
    }
}
