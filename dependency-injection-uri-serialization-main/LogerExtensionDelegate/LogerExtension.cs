using Microsoft.Extensions.Logging;

namespace LogerExtensionDelegate;
public static class LogerExtension
{
    private static Action<ILogger, string, Exception> fastLoggerMessage =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            eventId: new EventId(id: 1, name: "ERROR"),
            formatString: "Message: {Message}");

    public static void FastLoggerMessage(ILogger? logger, string message, Exception exception)
    {
        if (logger != null)
        {
            fastLoggerMessage(logger, message, exception);
        }
    }
}
