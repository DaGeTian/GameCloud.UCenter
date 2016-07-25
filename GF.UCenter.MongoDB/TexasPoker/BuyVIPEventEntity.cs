namespace GF.UCenter.MongoDB.TexasPoker
{
    using Attributes;
    using Entity;

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
