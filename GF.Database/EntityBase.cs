using System;

namespace GF.Database
{
    /// <summary>
    /// Provide a base entity class.
    /// </summary>
    public abstract class EntityBase
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the created time.
        /// </summary>
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the updated time.
        /// </summary>
        public DateTime UpdatedTime { get; set; } = DateTime.UtcNow;
    }
}
