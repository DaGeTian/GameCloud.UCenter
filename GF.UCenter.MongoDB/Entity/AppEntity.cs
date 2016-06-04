namespace GF.UCenter.MongoDB.Entity
{
    using Attributes;

    [CollectionName("App")]
    public class AppEntity : EntityBase
    {
        public string Name { get; set; }
        public string AppSecret { get; set; }
        public string Configuration { get; set; }
    }
}