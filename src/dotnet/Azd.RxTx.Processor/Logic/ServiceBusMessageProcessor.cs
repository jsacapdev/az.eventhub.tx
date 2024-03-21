namespace Azd.RxTx.Processor;

public class ServiceBusMessageProcessor : IMessageProcessor
{
    private readonly ILogger<ServiceBusMessageProcessor> _logger;

    public ServiceBusMessageProcessor(ILogger<ServiceBusMessageProcessor> logger, IHostApplicationLifetime hostApplicationLifetime)
    {
        _logger = logger;

        hostApplicationLifetime.ApplicationStopped.Register(() => StopProcessing());
    }

    public async Task StartProcessingAsync()
    {
        await Task.Run(() => { });

        _logger.LogInformation("ServiceBusMessageProcessor started processing at: {time}", DateTimeOffset.Now);
    }

    public void StopProcessing()
    {
        _logger.LogInformation("ServiceBusMessageProcessor stopped processing at: {time}", DateTimeOffset.Now);
    }
}
