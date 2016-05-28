using System.ComponentModel.Composition;

namespace GF.UCenter.Web.Common.Logger
{
    [Export("Trace.NLog", typeof(ILoggerAdapter))]
    public class NLoggerAdapter : ILoggerAdapter
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

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
