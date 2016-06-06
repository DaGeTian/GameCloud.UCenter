namespace GF.UCenter.MongoDB.Exceptions
{
    using System;

    /// <summary>
    /// Provide an exception for invalid version.
    /// </summary>
    public class InvalidVersionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidVersionException" /> class.
        /// </summary>
        /// <param name="type">Indicating the type value.</param>
        /// <param name="id">Indicating the id.</param>
        /// <param name="currentVersion">Indicating the current version number.</param>
        /// <param name="eventVersion">Indicating the event version number.</param>
        public InvalidVersionException(string type, Guid id, int currentVersion, int eventVersion)
            : base($"Updating {type} failed ({id}), current version = {currentVersion}, event version = {eventVersion}")
        {
        }
    }
}
