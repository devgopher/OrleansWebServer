using NLog;
using System;
using System.Linq;

namespace AsyncLogging
{
    public class NLogLogger : AsyncLogging, IAsyncLogger
    {
        private readonly Logger _logger;
        public NLogLogger(Logger logger) => _logger = logger;

        public NLogLogger() => _logger = LogManager.GetCurrentClassLogger();

        protected override void Log(IAsyncLogger.LogLevel level, string message, params object[] objs)
        {
            switch (level)
            {
                case IAsyncLogger.LogLevel.Trace:
                    _logger.Trace(message);
                    break;
                case IAsyncLogger.LogLevel.Debug:
                    _logger.Debug(message);
                    break;
                case IAsyncLogger.LogLevel.Info:
                    _logger.Info(message);
                    break;
                case IAsyncLogger.LogLevel.Warn:
                    _logger.Warn(message);
                    break;
                case IAsyncLogger.LogLevel.Error:
                    if (objs != default && !objs.Any())
                    {
                        if (objs[0] is Exception ex)
                        {
                            var args = objs.TakeLast(objs.Length - 1);
                            _logger.Error(ex, message, args);
                        }
                        else
                            _logger.Error(message, objs);
                    }
                    else
                        _logger.Error(message);

                    break;
                case IAsyncLogger.LogLevel.Fatal:
                    if (objs != default && !objs.Any())
                    {
                        if (objs[0] is Exception ex)
                        {
                            var args = objs.TakeLast(objs.Length - 1);
                            _logger.Fatal(ex, message, args);
                        }
                        else
                            _logger.Fatal(message, objs);
                    }
                    else
                        _logger.Fatal(message);

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

    }
}
