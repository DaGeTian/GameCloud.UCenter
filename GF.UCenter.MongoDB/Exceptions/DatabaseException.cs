namespace GF.UCenter.MongoDB.Exceptions
{
    using System;

    /// <summary>
    /// Provide an database exception class.
    /// </summary>
    public class DatabaseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseException" /> class.
        /// </summary>
        /// <param name="message">Indicating the exception message.</param>
        public DatabaseException(string message)
            : base($"The operation failed with {message}")
        {
        }
    }
}
