using GF.Database;
using GF.Database.Attributes;

namespace TexasPoker.Database
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
