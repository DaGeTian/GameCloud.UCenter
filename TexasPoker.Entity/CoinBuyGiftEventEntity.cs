using GF.MongoDB.Common;
using GF.MongoDB.Common.Attributes;

namespace TexasPoker.Entity
{
    [CollectionName("CoinBuyGiftEvent")]
    public class CoinBuyGiftEventEntity : EntityBase
    {
        public string BuyPlayerEtGuid { get; set; }
        public string Target { get; set; }
        public int GiftTbId { get; set; }
        public int CostCoins { get; set; }
        public int AfterBuyCoinsNum { get; set; }
        public string EventType { get; set; }
        public string EventTm { get; set; }
    }
}
