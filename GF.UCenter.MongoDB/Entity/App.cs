using GF.UCenter.MongoDB.Attributes;

namespace GF.UCenter.MongoDB.Entity
{
    [CollectionName("App")]
    public class App : EntityBase
    {
        public string Name { get; set; }
        public string AppSecret { get; set; }
        public string Configuration { get; set; }
    }
}