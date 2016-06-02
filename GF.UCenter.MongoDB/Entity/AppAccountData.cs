using GF.UCenter.MongoDB.Attributes;

namespace GF.UCenter.MongoDB.Entity
{
    [CollectionName("AppAccountData")]
    public class AppAccountData : EntityBase
    {
        public string AppId { get; set; }

        public string AccountId { get; set; }

        public string Data { get; set; }
    }
}