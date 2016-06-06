namespace GF.UCenter.MongoDB.Exceptions
{
    using System;

    /// <summary>
    /// Provide an exception for model locked.
    /// </summary>
    public class ModelLockedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelLockedException" /> class.
        /// </summary>
        /// <param name="type">Indicating the type value.</param>
        /// <param name="id">Indicating the id.</param>
        public ModelLockedException(string type, Guid id)
            : base($"Model {type} locked ({id})")
        {
        }
    }
}
