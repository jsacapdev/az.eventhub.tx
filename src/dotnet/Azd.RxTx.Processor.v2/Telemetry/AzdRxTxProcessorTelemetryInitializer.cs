using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Azd.RxTx.Processor.v2;

public class AzdRxTxProcessorTelemetryInitializer : ITelemetryInitializer
{
    private readonly string? _serviceName;

    public AzdRxTxProcessorTelemetryInitializer(IConfiguration configuration)
    {
        _serviceName = configuration["ServiceName"];
    }

    public void Initialize(ITelemetry telemetry)
    {
        var requestTelemetry = telemetry as RequestTelemetry;

        if (requestTelemetry == null) return;

        requestTelemetry.Context.Cloud.RoleName = _serviceName;
    }
}
