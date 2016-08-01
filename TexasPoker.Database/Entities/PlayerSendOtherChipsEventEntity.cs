using GF.Database;
using GF.Database.Attributes;

namespace TexasPoker.Database
{
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
