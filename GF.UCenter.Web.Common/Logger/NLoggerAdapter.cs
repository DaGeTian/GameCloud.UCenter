namespace GF.UCenter.Web.Common.Logger
{
    using System.ComponentModel.Composition;
    using NLog;

    [Export("Trace.NLog", typeof(ILoggerAdapter))]
    public class NLoggerAdapter : ILoggerAdapter
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public void TraceError(string message)
        {
            logger.Error(message);
        }

        public void TraceInformation(string message)
        {
            logger.Info(message);
        }

        public void TraceWarning(string message)
        {
            logger.Warn(message);
        }
    }
}
