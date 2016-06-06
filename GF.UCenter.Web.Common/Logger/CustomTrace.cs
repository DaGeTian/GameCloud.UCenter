namespace GF.UCenter.Web.Common.Logger
{
    using System;
    using System.ComponentModel.Composition.Hosting;
    using System.Globalization;

    /// <summary>
    /// Provide a class used for custom trace.
    /// </summary>
    public static class CustomTrace
    {
        private static ILoggerAdapter adapter = new NLoggerAdapter();

        /// <summary>
        /// Initialize the custom trace.
        /// </summary>
        /// <param name="exportProvider">Indicating the export provider.</param>
        /// <param name="adapterType">Indicating the adapter type.</param>
        public static void Initialize(ExportProvider exportProvider, string adapterType)
        {
            adapter = exportProvider.GetExportedValue<ILoggerAdapter>(adapterType);
        }

        /// <summary>
        /// Trace information message.
        /// </summary>
        /// <param name="message">Indicating the message.</param>
        /// <param name="args">Indicating the message arguments.</param>
        public static void TraceInformation(string message, params object[] args)
        {
            adapter.TraceInformation(FormatMessage(message, args));
        }

        /// <summary>
        /// Trace warning message.
        /// </summary>
        /// <param name="message">Indicating the message.</param>
        /// <param name="args">Indicating the message arguments.</param>
        public static void TraceWarning(string message, params object[] args)
        {
            adapter.TraceWarning(FormatMessage(message, args));
        }

        /// <summary>
        /// Trace error message.
        /// </summary>
        /// <param name="exception">Indicating the exception.</param>
        /// <param name="message">Indicating the message.</param>
        /// <param name="args">Indicating the message arguments.</param>
        public static void TraceError(Exception exception, string message, params object[] args)
        {
            if (string.IsNullOrEmpty(message))
            {
                message = exception.ToString();
            }
            else
            {
                message = FormatMessage(message, args);
                if (exception != null)
                {
                    message = FormatMessage("{0}, Exception: {1}", message, exception);
                }
            }

            adapter.TraceError(message);
        }

        /// <summary>
        /// Trace error message.
        /// </summary>
        /// <param name="exception">Indicating the exception.</param>
        public static void TraceError(Exception exception)
        {
            TraceError(exception, null);
        }

        /// <summary>
        /// Trace error message.
        /// </summary>
        /// <param name="message">Indicating the message.</param>
        /// <param name="args">Indicating the message arguments.</param>
        public static void TraceError(string message, params object[] args)
        {
            TraceError(null, message, args);
        }

        private static string FormatMessage(string messageTemplate, params object[] args)
        {
            string message = messageTemplate;
            if (args != null && args.Length > 0)
            {
                message = string.Format(CultureInfo.InvariantCulture, messageTemplate, args);
            }

            return message;
        }
    }
}
