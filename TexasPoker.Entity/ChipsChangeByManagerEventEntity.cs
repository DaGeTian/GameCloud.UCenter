using GF.MongoDB.Common;
using GF.MongoDB.Common.Attributes;

namespace TexasPoker.Entity
{
    [CollectionName("ChipsChangeByManagerEvent")]
    public class ChipsChangeByManagerEventEntity : EntityBase
    {
        public string PlayerEtGuid { get; set; }
        public int ChangeChips { get; set; }
        public int AfterChangeChipsNum { get; set; }
        public string EventType { get; set; }
        public string EventTm { get; set; }
    }
}
