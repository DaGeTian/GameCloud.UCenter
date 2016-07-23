namespace GF.UCenter.MongoDB.TexasPoker
{
    using Entity;

    public class PlayerGetChipsFromOtherEventEntity : EntityBase
    {
        public string EventType;
        public string GetChipsPlayerEtGuid;
        public string FromPlayerEtGuid;
        public string GetChipsNum;
        public string AfterGetChipsNum;
        public string EventTm;
    }
}
