using System;
using System.Threading.Tasks;

namespace AsyncLogging
{
    public abstract class AsyncLogging : IAsyncLogger
    {
        public void Trace(string message) => Task.Run(() => Log(IAsyncLogger.LogLevel.Trace, message));

        public void Debug(string message) => Task.Run(() => Log(IAsyncLogger.LogLevel.Debug, message));

        public void Info(string message) => Task.Run(() => Log(IAsyncLogger.LogLevel.Info, message));

        public void Warn(string message) => Task.Run(() => Log(IAsyncLogger.LogLevel.Warn, message));

        public void Error(string message, Exception ex = default) => Task.Run(() => Log(IAsyncLogger.LogLevel.Error, message, ex));

        public void Fatal(string message, Exception ex = default) => Task.Run(() => Log(IAsyncLogger.LogLevel.Fatal, message, ex));

        protected abstract void Log(IAsyncLogger.LogLevel level, string message, params object[] objs);
    }
}
