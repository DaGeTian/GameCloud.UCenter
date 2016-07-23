namespace GF.UCenter.MongoDB.TexasPoker
{
    using Entity;

    public class CoinBuyItemEventEntity : EntityBase
    {
        public string EventType;
        public string BuyPlayerEtGuid;
        public string GiftTbId;
        public string CostCoins;
        public string AfterBuyCoinsNum;
        public string EventTm;
    }
}
