using System;
using GameCloud.Database;
using GameCloud.Database.Attributes;
using GameCloud.UCenter.Common.Portable.Contracts;

namespace GameCloud.UCenter.Database.Entities
{
    /// <summary>
    /// Provide an account event entity class.
    /// </summary>
    [CollectionName("ExceptionEvent")]
    public class ExceptionEventEntity : EntityBase
    {
        /// <summary>
        /// Gets or sets the exception message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the exception stack trace.
        /// </summary>
        public string StackTrace { get; set; }
    }
}
