namespace GF.UCenter.MongoDB.TexasPoker
{
    using Attributes;
    using Entity;

    [CollectionName("DailyGetChipsEvent")]
    public class DailyGetChipsEventEntity : EntityBase
    {
        public string GetPlayerEtGuid { get; set; }
        public int GetChipsNum { get; set; }
        public int AfterGetChipsNum { get; set; }
        public string EventType { get; set; }
        public string EventTm { get; set; }
    }
}
