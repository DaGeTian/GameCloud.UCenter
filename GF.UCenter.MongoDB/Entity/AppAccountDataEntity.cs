namespace GF.UCenter.MongoDB.Entity
{
    using Attributes;

    [CollectionName("AppAccountData")]
    public class AppAccountDataEntity : EntityBase
    {
        public string AppId { get; set; }

        public string AccountId { get; set; }

        public string Data { get; set; }
    }
}