using System;
using System.ComponentModel.Composition;

namespace GF.UCenter.Web.Common.Logger
{
    [Export("Trace.Console", typeof(ILoggerAdapter))]
    public class ConsoleLoggerAdapter : ILoggerAdapter
    {
        private static object locker = new object();

        public void TraceError(string message)
        {
            this.WriteMessage(ConsoleColor.Red, message);
        }

        public void TraceInformation(string message)
        {
            this.WriteMessage(ConsoleColor.Gray, message);
        }

        public void TraceWarning(string message)
        {
            this.WriteMessage(ConsoleColor.Yellow, message);
        }

        private void WriteMessage(ConsoleColor color, string message)
        {
            lock (locker)
            {
                var pColor = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine(message);
                Console.ForegroundColor = pColor;
            }
        }
    }
}
