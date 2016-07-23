namespace GF.UCenter.MongoDB.TexasPoker
{
    using Entity;

    public class PlayerReportEventEntity : EntityBase
    {
        public string EventType;
        public string EventTm;
        public string ReportPlayer;
        public string BeingReportedPlayer;
        public string ReportType;
    }
}
