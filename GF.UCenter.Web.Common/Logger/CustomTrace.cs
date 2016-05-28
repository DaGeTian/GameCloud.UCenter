using System;
using System.ComponentModel.Composition.Hosting;
using System.Globalization;

namespace GF.UCenter.Web.Common.Logger
{
    public static class CustomTrace
    {
        private static ILoggerAdapter adapter = new NLoggerAdapter();

        public static void Initialize(ExportProvider exportProvider, string adapterType)
        {
            adapter = exportProvider.GetExportedValue<ILoggerAdapter>(adapterType);
        }

        public static void TraceInformation(string message, params object[] args)
        {
            adapter.TraceInformation(FormatMessage(message, args));
        }

        public static void TraceWarning(string message, params object[] args)
        {
            adapter.TraceWarning(FormatMessage(message, args));
        }

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

        public static void TraceError(Exception exception)
        {
            TraceError(exception, null);
        }

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
