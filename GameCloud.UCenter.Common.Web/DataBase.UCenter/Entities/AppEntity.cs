using GameCloud.Database;
using GameCloud.Database.Attributes;

namespace GameCloud.UCenter.Database.Entities
{
    [CollectionName("App")]
    public class AppEntity : EntityBase
    {
        public string Name { get; set; }
        public string AppSecret { get; set; }
        public string WechatAppId { get; set; }
        public string WechatAppSecret { get; set; }
    }
}
