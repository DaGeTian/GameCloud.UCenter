namespace GF.UCenter.MongoDB.TexasPoker
{
    using Attributes;
    using Entity;

    [CollectionName("PlayerSendOtherChipsEvent")]
    public class PlayerSendOtherChipsEventEntity : EntityBase
    {
        public string SendPlayerEtGuid { get; set; }
        public string TargetPlayerEtGuid { get; set; }
        public int SendChipsNum { get; set; }
        public int AfterSendChipsNum { get; set; }
        public string EventType { get; set; }
        public string EventTm { get; set; }
    }
}
