namespace GF.UCenter.Web.Common.Logger
{
    /// <summary>
    /// Provide an interface for log adapter.
    /// </summary>
    public interface ILoggerAdapter
    {
        /// <summary>
        /// Trace error message.
        /// </summary>
        /// <param name="message">Indicating the message.</param>
        void TraceError(string message);

        /// <summary>
        /// Trace information message.
        /// </summary>
        /// <param name="message">Indicating the message.</param>
        void TraceInformation(string message);

        /// <summary>
        /// Trace warning message.
        /// </summary>
        /// <param name="message">Indicating the message.</param>
        void TraceWarning(string message);
    }
}
