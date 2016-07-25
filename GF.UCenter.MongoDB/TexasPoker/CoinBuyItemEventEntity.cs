namespace GF.UCenter.MongoDB.TexasPoker
{
    using Attributes;
    using Entity;

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
