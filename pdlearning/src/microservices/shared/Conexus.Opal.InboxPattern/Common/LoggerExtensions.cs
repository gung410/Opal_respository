using System;
using Microsoft.Extensions.Logging;

namespace Conexus.Opal.InboxPattern.Common
{
    /// <summary>
    /// This is one my favorite code patterns.
    /// See more at: https://github.com/dotnet/aspnetcore/blob/master/src/Middleware/ResponseCaching/src/LoggerExtensions.cs.
    /// and figures out by yourself why we should use this.
    /// </summary>
    internal static class LoggerExtensions
    {
        private static readonly Action<ILogger, Exception> _concurrencyExceptionWhenGettingMessageFromQueue =
            LoggerMessage.Define(LogLevel.Warning, new EventId(1, "ConcurrencyExceptionWhenGettingMessageFromQueue"), "There is a concurrency warning when getting messages from queue.");

        private static readonly Action<ILogger, Exception> _concurrencyExceptionWhenTryingToPrepareMessagesForSending =
            LoggerMessage.Define(LogLevel.Warning, new EventId(2, "ConcurrencyExceptionWhenTryingToPrepareMessagesForSending"), "There is a concurrency warning when preparing messages to send.");

        private static readonly Action<ILogger, Exception> _noMessageAvailableFromTheOutboxQueue =
            LoggerMessage.Define(LogLevel.Information, new EventId(3, "NoMessageAvailableFromTheOutboxQueue"), "There is no messages available from the outbox queue.");

        private static readonly Action<ILogger, int, Exception> _numberOfCleanedMessages =
            LoggerMessage.Define<int>(LogLevel.Information, new EventId(4, "NumberOfCleanedMessages"), "The cleaner has cleaned {MessageCount} message(s).");

        private static readonly Action<ILogger, Exception> _exceptionWhenDeleteMessageFromQueue =
            LoggerMessage.Define(LogLevel.Warning, new EventId(5, "ExceptionWhenDeleteMessageFromQueue"), "There is an exception when deleting messages from queue.");

        internal static void ConcurrencyExceptionWhenGettingMessageFromQueue(this ILogger logger, Exception exception)
        {
            _concurrencyExceptionWhenGettingMessageFromQueue(logger, exception);
        }

        internal static void ConcurrencyExceptionWhenTryingToPrepareMessagesForSending(this ILogger logger, Exception exception)
        {
            _concurrencyExceptionWhenTryingToPrepareMessagesForSending(logger, exception);
        }

        internal static void NoMessageAvailableFromTheOutboxQueue(this ILogger logger)
        {
            _noMessageAvailableFromTheOutboxQueue(logger, null);
        }

        internal static void NumberOfCleanedMessages(this ILogger logger, int messageCount)
        {
            _numberOfCleanedMessages(logger, messageCount, null);
        }

        internal static void ExceptionWhenDeleteMessageFromQueue(this ILogger logger, Exception exception)
        {
            _exceptionWhenDeleteMessageFromQueue(logger, exception);
        }
    }
}
