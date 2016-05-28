namespace GF.UCenter.Web.Common.Logger
{
    public interface ILoggerAdapter
    {
        void TraceError(string message);

        void TraceInformation(string message);

        void TraceWarning(string message);
    }
}
