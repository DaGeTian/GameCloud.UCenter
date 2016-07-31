using GF.MongoDB.Common;
using GF.MongoDB.Common.Attributes;

namespace TexasPoker.Entity
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
