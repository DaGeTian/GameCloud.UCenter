namespace GF.UCenter.MongoDB.TexasPoker
{
    using Entity;

    public class PlayerSendOtherChipsEventEntity : EntityBase
    {
        public string EventType;
        public string SendPlayerEtGuid;
        public string TargetPlayerEtGuid;
        public string SendChipsNum;
        public string AfterSendChipsNum;
        public string EventTm;
    }
}
