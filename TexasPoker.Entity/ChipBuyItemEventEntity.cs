using GF.Database.Entity.Common;
using GF.Database.Entity.Common.Attributes;

namespace TexasPoker.Entity
{
    [CollectionName("ChipBuyItemEvent")]
    public class ChipBuyItemEventEntity : EntityBase
    {
        public string BuyPlayerEtGuid { get; set; }
        public int GiftTbId { get; set; }
        public int CostChips { get; set; }
        public int AfterBuyChipsNum { get; set; }
        public string EventType { get; set; }
        public string EventTm { get; set; }
    }
}
