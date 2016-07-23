namespace GF.UCenter.MongoDB.TexasPoker
{
    using Entity;

    public class ChipBuyGiftEventEntity : EntityBase
    {
        public string EventType;
        public string BuyPlayerEtGuid;
        public string Target;
        public string GiftTbId;
        public string CostChips;
        public string AfterBuyChipsNum;
        public string EventTm;
    }
}
