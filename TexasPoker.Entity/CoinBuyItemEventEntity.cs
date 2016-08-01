using GF.Database.Entity.Common;
using GF.Database.Entity.Common.Attributes;

namespace TexasPoker.Entity
{
    [CollectionName("CoinBuyItemEvent")]
    public class CoinBuyItemEventEntity : EntityBase
    {
        public string BuyPlayerEtGuid { get; set; }
        public int GiftTbId { get; set; }
        public int CostCoins { get; set; }
        public int AfterBuyCoinsNum { get; set; }
        public string EventType { get; set; }
        public string EventTm { get; set; }
    }
}
