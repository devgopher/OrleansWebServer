using System;

namespace AsyncLogging
{
    public class ConsoleLogger : AsyncLogging, IAsyncLogger
    {
        public ConsoleLogger() { }

        protected override void Log(IAsyncLogger.LogLevel level, string message, params object[] objs)
        {
            switch (level)
            {
                case IAsyncLogger.LogLevel.Trace:
                    Console.WriteLine($"TRACE: {message}");
                    break;
                case IAsyncLogger.LogLevel.Debug:
                    Console.WriteLine($"DEBUG: {message}");
                    break;
                case IAsyncLogger.LogLevel.Info:
                    Console.WriteLine($"INFO: {message}");
                    break;
                case IAsyncLogger.LogLevel.Warn:
                    Console.WriteLine($"WARN: {message}");
                    break;
                case IAsyncLogger.LogLevel.Error:
                    Console.WriteLine($"ERROR: {message}");
                    break;
                case IAsyncLogger.LogLevel.Fatal:
                    Console.WriteLine($"FATAL: {message}");
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }
    }
}