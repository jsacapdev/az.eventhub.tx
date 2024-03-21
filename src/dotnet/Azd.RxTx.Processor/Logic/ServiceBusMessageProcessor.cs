namespace Azd.RxTx.Processor;

public class ServiceBusMessageProcessor : IMessageProcessor
{
    private readonly ILogger<ServiceBusMessageProcessor> _logger;

    public ServiceBusMessageProcessor(ILogger<ServiceBusMessageProcessor> logger)
    {
        _logger = logger;
    }

    public async Task StartProcessingAsync()
    {
        await Task.Run(() => {});

        _logger.LogInformation("ServiceBusMessageProcessor started processing at: {time}", DateTimeOffset.Now);
    }

    public async Task StopProcessingAsync()
    {
        await Task.Run(() => {});

        _logger.LogInformation("ServiceBusMessageProcessor stopped processing at: {time}", DateTimeOffset.Now);
    }
}
