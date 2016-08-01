using GF.Database;
using GF.Database.Attributes;

namespace TexasPoker.Database
{
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
