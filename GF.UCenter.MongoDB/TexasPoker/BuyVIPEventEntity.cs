namespace GF.UCenter.MongoDB.TexasPoker
{
    using Entity;

    public class BuyVIPEventEntity : EntityBase
    {
        public string EventType;
        public string BuyPlayerEtGuid;
        public string BuyVIPDays;
        public string AfterBuyDays;
        public string CostMoney;
        public string EventTm;
    }
}
