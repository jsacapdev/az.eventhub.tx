namespace Azd.RxTx.Processor;

public static class Extensions
{
    public static void LogTimestampedInformation<T>(this ILogger<T> logger, string message, params object?[] args)
    {
        logger.LogInformation(message, args, DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", System.Globalization.CultureInfo.InvariantCulture));
    }
}
