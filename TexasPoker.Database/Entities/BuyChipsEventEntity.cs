using GF.Database;
using GF.Database.Attributes;

namespace TexasPoker.Database
{
    [CollectionName("BuyChipsEvent")]
    public class BuyChipsEventEntity : EntityBase
    {
        public string BuyPlayerEtGuid { get; set; }
        public int BuyNum { get; set; }
        public int AfterBuyNum { get; set; }
        public int CostMoney { get; set; }
        public string EventType { get; set; }
        public string EventTm { get; set; }
    }
}
