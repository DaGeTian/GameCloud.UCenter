using GF.MongoDB.Common;
using GF.MongoDB.Common.Attributes;

namespace GF.Manager.Models
{
    [CollectionName("Plugin")]
    public class PluginEntity : EntityBase
    {
        public string Name { get; set; }

        public string Content { get; set; }
    }
}