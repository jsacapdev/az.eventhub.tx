using Azure.Messaging.ServiceBus;

namespace Azd.RxTx.Processor;

public class ServiceBusMessageProcessor : IMessageProcessor
{
    private readonly ILogger<ServiceBusMessageProcessor> _logger;

    private readonly IMessageSender<string> _messageForwarder;


    private readonly ServiceBusProcessor _serviceBusProcessor;

    public ServiceBusMessageProcessor(ILogger<ServiceBusMessageProcessor> logger,
                                      IMessageSender<string> messageForwarder,
                                      IHostApplicationLifetime hostApplicationLifetime,
                                      ServiceBusClient serviceBusClient)
    {
        _logger = logger;

        _messageForwarder = messageForwarder;

        _serviceBusProcessor = serviceBusClient.CreateProcessor("sbt-azd5-dev-001", "sub-azd5-dev-001");

        hostApplicationLifetime.ApplicationStopped.Register(async () => await StopProcessing());
    }

    public void Initialize()
    {
        _serviceBusProcessor.ProcessMessageAsync += ServiceBusMessageHandler;

        _serviceBusProcessor.ProcessErrorAsync += ServiceBusErrorHandler;

        _logger.LogInformation("ServiceBusMessageProcessor completed Initialization at: {time}", DateTimeOffset.Now);
    }

    private async Task ServiceBusMessageHandler(ProcessMessageEventArgs args)
    {
        _logger.LogInformation("ServiceBusMessageProcessor received new message for processing at: {time}",
                              DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", System.Globalization.CultureInfo.InvariantCulture));

        string body = args.Message.Body.ToString();

        await _messageForwarder.SendMessagesAsync([body]);

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
