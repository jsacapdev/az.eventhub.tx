using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace Azd.RxTx.Processor;

public static class Extensions
{
    public static void TrackEvent(this TelemetryClient telemetryClient, ILogger logger, string message)
    {
        using (telemetryClient.StartOperation<RequestTelemetry>("operation"))
        {
            telemetryClient.TrackEvent(message);
        }

        logger.LogInformation(message);
    }

    public static void TrackTrace(this TelemetryClient telemetryClient, ILogger logger, string message)
    {
        using (telemetryClient.StartOperation<RequestTelemetry>("operation"))
        {
            telemetryClient.TrackTrace(message);
        }

        logger.LogInformation(message);
    }

    public static void TrackException(this TelemetryClient telemetryClient, ILogger logger, Exception ex)
    {
        using (telemetryClient.StartOperation<RequestTelemetry>("operation"))
        {
            telemetryClient.TrackException(ex);
        }

        logger.LogInformation(ex.ToString());
    }
}
