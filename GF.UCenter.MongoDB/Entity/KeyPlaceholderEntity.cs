namespace GF.UCenter.MongoDB.Entity
{
    using Attributes;

    [CollectionName("KeyPlaceholder")]
    public class KeyPlaceholderEntity : EntityBase
    {
        public string Name { get; set; }

        public KeyType Type { get; set; }

        public string AccountId { get; set; }

        public string AccountName { get; set; }
    }

    public enum KeyType
    {
        Name,
        Phone,
        Email
    }
}
