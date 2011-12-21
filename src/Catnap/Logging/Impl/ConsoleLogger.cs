using System;

namespace Catnap.Logging.Impl
{
    public class ConsoleLogger : ILogger
    {
        public void LogMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}