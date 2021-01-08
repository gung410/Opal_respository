using System;
using Microsoft.Extensions.Logging;

namespace Thunder.Platform.AspNetCore.ExceptionHandling
{
    /// <summary>
    /// This pattern was inspired by https://github.com/dotnet/aspnetcore/blob/master/src/SignalR/clients/csharp/Http.Connections.Client/src/HttpConnection.Log.cs.
    /// </summary>
    public partial class ThunderExceptionFilter
    {
        private static class Log
        {
            private static readonly Action<ILogger, Exception> _generalRequestError =
                LoggerMessage.Define(LogLevel.Error, new EventId(1, "GeneralRequestError"), "There is an exception during the processing of the request");

            private static readonly Action<ILogger, string, Exception> _knownRequestError =
                LoggerMessage.Define<string>(LogLevel.Error, new EventId(2, "KnownRequestError"), "There is a {ExceptionType} during the processing of the request.");

            public static void GeneralRequestError(ILogger logger, Exception exception)
            {
                _generalRequestError(logger, exception);
            }

            public static void KnownRequestError(ILogger logger, string exceptionType, Exception exception)
            {
                _knownRequestError(logger, exceptionType, exception);
            }
        }
    }
}
