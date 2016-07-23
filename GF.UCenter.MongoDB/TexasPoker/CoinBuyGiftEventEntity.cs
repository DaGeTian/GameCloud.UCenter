namespace GF.UCenter.MongoDB.TexasPoker
{
    using Entity;

    public class CoinBuyGiftEventEntity : EntityBase
    {
        public string EventType;
        public string BuyPlayerEtGuid;
        public string Target;
        public string GiftTbId;
        public string CostCoins;
        public string AfterBuyCoinsNum;
        public string EventTm;
    }
}
