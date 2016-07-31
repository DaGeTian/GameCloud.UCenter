using GF.MongoDB.Common;
using GF.MongoDB.Common.Attributes;

namespace TexasPoker.Entity
{
    [CollectionName("BuyVIPEvent")]
    public class BuyVIPEventEntity : EntityBase
    {
        public string BuyPlayerEtGuid { get; set; }
        public int BuyVIPDays { get; set; }
        public int AfterBuyDays { get; set; }
        public int CostMoney { get; set; }
        public string EventType { get; set; }
        public string EventTm { get; set; }
    }
}
