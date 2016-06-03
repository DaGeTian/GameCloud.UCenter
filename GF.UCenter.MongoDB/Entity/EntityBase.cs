namespace GF.UCenter.MongoDB.Entity
{
    using System;

    public abstract class EntityBase
    {
        public virtual string Id { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedTime { get; set; } = DateTime.UtcNow;
    }
}
