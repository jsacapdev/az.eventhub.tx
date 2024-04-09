using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace Azd.RxTx.Processor.v2;

public static class Extensions
{
    public static void TrackEvent(this TelemetryClient telemetryClient, ILogger logger, string message)
    {
        telemetryClient.TrackEvent(message);

        logger.LogInformation(message);
    }

    public static void TrackTrace(this TelemetryClient telemetryClient, ILogger logger, string message)
    {
        telemetryClient.TrackTrace(message);

        logger.LogInformation(message);
    }

    public static void TrackException(this TelemetryClient telemetryClient, ILogger logger, Exception ex)
    {
        telemetryClient.TrackException(ex);

        logger.LogInformation(ex.ToString());
    }
}
