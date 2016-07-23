namespace GF.UCenter.MongoDB.TexasPoker
{
    using Entity;

    public class BuyCoinsEventEntity : EntityBase
    {
        public string EventType;
        public string BuyPlayerEtGuid;
        public string BuyNum;
        public string AfterBuyNum;
        public string CostMoney;
        public string EventTm;
    }
}
