using Azure.Messaging.ServiceBus;

namespace Azd.RxTx.Processor;

public class ServiceBusMessageProcessor : IMessageProcessor
{
    private readonly ILogger<ServiceBusMessageProcessor> _logger;

    private readonly ServiceBusProcessor _serviceBusProcessor;

    public ServiceBusMessageProcessor(ILogger<ServiceBusMessageProcessor> logger,
                                      IHostApplicationLifetime hostApplicationLifetime,
                                      ServiceBusClient serviceBusClient)
    {
        _logger = logger;

        _serviceBusProcessor = serviceBusClient.CreateProcessor("sbt-azd5-dev-001", "sub-azd5-dev-001");

        hostApplicationLifetime.ApplicationStopped.Register(async () => await StopProcessing());
    }

    public void Initialize()
    {
        _logger.LogInformation("ServiceBusMessageProcessor completed Initialization at at: {time}", DateTimeOffset.Now);
    }

    public async Task StartProcessingAsync()
    {
        await Task.Run(() => { });

        _logger.LogInformation("ServiceBusMessageProcessor started processing at: {time}", DateTimeOffset.Now);
    }

    public async Task StopProcessing()
    {
        await _serviceBusProcessor.DisposeAsync();

        _logger.LogInformation("ServiceBusMessageProcessor stopped processing at: {time}", DateTimeOffset.Now);
    }
}
