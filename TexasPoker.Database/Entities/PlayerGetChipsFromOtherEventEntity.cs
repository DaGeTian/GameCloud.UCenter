using GF.Database;
using GF.Database.Attributes;

namespace TexasPoker.Database
{
    [CollectionName("PlayerGetChipsFromOtherEvent")]
    public class PlayerGetChipsFromOtherEventEntity : EntityBase
    {
        public string GetChipsPlayerEtGuid { get; set; }
        public string FromPlayerEtGuid { get; set; }
        public int GetChipsNum { get; set; }
        public int AfterGetChipsNum { get; set; }
        public string EventType { get; set; }
        public string EventTm { get; set; }
    }
}
