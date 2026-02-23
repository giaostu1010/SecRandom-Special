using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;

namespace SecRandom.Core.Services.Logging;

public class SecRandomFileLoggerProvider : ILoggerProvider
{
    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();
        public void Dispose() { }
    }

    private readonly string _logFilePath;
    private readonly object _lock = new();

    public SecRandomFileLoggerProvider(string logFilePath)
    {
        _logFilePath = logFilePath;
        var directory = Path.GetDirectoryName(_logFilePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new SecRandomFileLogger(categoryName, _logFilePath, _lock);
    }

    public void Dispose()
    {
    }

    private class SecRandomFileLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly string _logFilePath;
        private readonly object _lock;

        public SecRandomFileLogger(string categoryName, string logFilePath, object @lock)
        {
            _categoryName = categoryName;
            _logFilePath = logFilePath;
            _lock = @lock;
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var message = formatter(state, exception);
            var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{logLevel}] [{_categoryName}] {message}";

            if (exception != null)
            {
                logEntry += Environment.NewLine + exception;
            }

            lock (_lock)
            {
                File.AppendAllText(_logFilePath, logEntry + Environment.NewLine, Encoding.UTF8);
            }
        }
    }
}
