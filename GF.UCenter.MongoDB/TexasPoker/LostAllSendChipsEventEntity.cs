namespace GF.UCenter.MongoDB.TexasPoker
{
    using Entity;

    public class LostAllSendChipsEventEntity : EntityBase
    {
        public string EventType;
        public string GetPlayerEtGuid;
        public string GetChipsNum;
        public string AfterGetChipsNum;
        public string EventTm;
    }
}
