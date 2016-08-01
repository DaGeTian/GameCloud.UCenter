using GF.Database;
using GF.Database.Attributes;

namespace TexasPoker.Database
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
