namespace GF.UCenter.MongoDB.TexasPoker
{
    using Entity;

    public class ChipBuyItemEventEntity : EntityBase
    {
        public string EventType;
        public string BuyPlayerEtGuid;
        public string GiftTbId;
        public string CostChips;
        public string AfterBuyChipsNum;
        public string EventTm;
    }
}
