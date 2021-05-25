using System;

namespace AsyncLogging
{
    public interface IAsyncLogger
    {
        public enum LogLevel
        {
            Trace,
            Debug,
            Info,
            Warn,
            Error,
            Fatal
        }

        void Trace(string message);
        void Debug(string message);
        void Info(string message);
        void Warn(string message);
        void Error(string message, Exception ex = default);
        void Fatal(string message, Exception ex = default);
    }
}
 