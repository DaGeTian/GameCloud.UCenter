using GF.Database.Entity.Common;
using GF.Database.Entity.Common.Attributes;

namespace TexasPoker.Entity
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
