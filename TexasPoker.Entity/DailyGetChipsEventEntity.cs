using GF.Database.Entity.Common;
using GF.Database.Entity.Common.Attributes;

namespace TexasPoker.Entity
{
    [CollectionName("DailyGetChipsEvent")]
    public class DailyGetChipsEventEntity : EntityBase
    {
        public string GetPlayerEtGuid { get; set; }
        public int GetChipsNum { get; set; }
        public int AfterGetChipsNum { get; set; }
        public string EventType { get; set; }
        public string EventTm { get; set; }
    }
}
