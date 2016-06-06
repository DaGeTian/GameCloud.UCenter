namespace GF.UCenter.Web.Common.Logger
{
    using System;
    using System.ComponentModel.Composition;

    /// <summary>
    /// Provide a class for console log adapter.
    /// </summary>
    [Export("Trace.Console", typeof(ILoggerAdapter))]
    public class ConsoleLoggerAdapter : ILoggerAdapter
    {
        private static readonly object Locker = new object();

        /// <summary>
        /// Trace error message.
        /// </summary>
        /// <param name="message">Indicating the message.</param>
        public void TraceError(string message)
        {
            this.WriteMessage(ConsoleColor.Red, "[ERROR]", message);
        }

        /// <summary>
        /// Trace information message.
        /// </summary>
        /// <param name="message">Indicating the message.</param>
        public void TraceInformation(string message)
        {
            this.WriteMessage(ConsoleColor.Gray, "[INFORMATION]", message);
        }

        /// <summary>
        /// Trace warning message.
        /// </summary>
        /// <param name="message">Indicating the message.</param>
        public void TraceWarning(string message)
        {
            this.WriteMessage(ConsoleColor.Yellow, "[WARNING]", message);
        }

        private void WriteMessage(ConsoleColor color, string prefix, string message)
        {
            lock (Locker)
            {
                var preColor = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine($"{ prefix }{ message }");
                Console.ForegroundColor = preColor;
            }
        }
    }
}
