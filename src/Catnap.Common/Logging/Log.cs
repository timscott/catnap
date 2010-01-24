using System;

namespace Catnap.Common.Logging
{
    public static class Log
    {
        public static LogLevel Level = LogLevel.Off;
        public static ILogger ConsoleLogger = new ConsoleLogger();
        public static ILogger FileLogger;

        public static void Debug(string message, params object[] args)
        {
            if (Level > LogLevel.Debug) return;
            LogMessage(message, args);
        }

        public static void Info(string message, params object[] args)
        {
            if (Level > LogLevel.Info) return;
            LogMessage(message, args);
        }

        public static void Error(Exception ex)
        {
            if (Level > LogLevel.Error) return;
            var stackTrace = ex.StackTrace;
            var message = ex.Message;
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                message += " " + ex.Message;
            }
            LogMessage(message + "\r\n" + stackTrace);
        }

        public static void Error(string message, params object[] args)
        {
            if (Level > LogLevel.Error) return;
            LogMessage(message, args);
        }

        private static void LogMessage(string message, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                message = string.Format(message, args);
            }
            if (ConsoleLogger != null)
            {
                ConsoleLogger.LogMessage(message);
            }
            if (FileLogger != null)
            {
                FileLogger.LogMessage(message);
            }
        }
    }
}