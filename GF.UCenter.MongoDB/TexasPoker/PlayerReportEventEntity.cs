namespace GF.UCenter.MongoDB.TexasPoker
{
    using Attributes;
    using Entity;

    [CollectionName("PlayerReportEvent")]
    public class PlayerReportEventEntity : EntityBase
    {
        public string ReportPlayer { get; set; }
        public string BeingReportedPlayer { get; set; }
        public string ReportType { get; set; }
        public string EventType { get; set; }
        public string EventTm { get; set; }
    }
}
