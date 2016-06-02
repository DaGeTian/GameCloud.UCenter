namespace GF.UCenter.MongoDB.Entity
{
    using System;

    public abstract class EntityBase
    {
        public string Id { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
    }
}
