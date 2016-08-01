using GF.Database;
using GF.Database.Attributes;

namespace TexasPoker.Database
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
