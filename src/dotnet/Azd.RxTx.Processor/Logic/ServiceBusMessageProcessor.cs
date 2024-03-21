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
        _serviceBusProcessor.ProcessMessageAsync += ServiceBusMessageHandler;

        _serviceBusProcessor.ProcessErrorAsync += ServiceBusErrorHandler;

        _logger.LogInformation("ServiceBusMessageProcessor completed Initialization at at: {time}", DateTimeOffset.Now);
    }

    private async Task ServiceBusMessageHandler(ProcessMessageEventArgs args)
    {
        string body = args.Message.Body.ToString();

        Console.WriteLine($"Received: {body}.");

        // complete (and so delete) the message. 
        await args.CompleteMessageAsync(args.Message);
    }

    private Task ServiceBusErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception.ToString());

        return Task.CompletedTask;
    }

    public async Task StartProcessingAsync()
    {
        await _serviceBusProcessor.StartProcessingAsync();

        _logger.LogInformation("ServiceBusMessageProcessor started processing at: {time}", DateTimeOffset.Now);
    }

    public async Task StopProcessing()
    {
        await _serviceBusProcessor.StopProcessingAsync();

        await _serviceBusProcessor.DisposeAsync();

        _logger.LogInformation("ServiceBusMessageProcessor stopped processing at: {time}", DateTimeOffset.Now);
    }
}
