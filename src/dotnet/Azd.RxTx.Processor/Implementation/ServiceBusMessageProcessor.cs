using Azure.Messaging.ServiceBus;
using Microsoft.ApplicationInsights;

namespace Azd.RxTx.Processor;

public class ServiceBusMessageProcessor : IMessageProcessor
{
    private readonly ILogger<ServiceBusMessageProcessor> _logger;

    private readonly IMessageSender<string> _messageForwarder;

    private readonly ServiceBusProcessor _serviceBusProcessor;

    private readonly TelemetryClient _telemetryClient;

    public ServiceBusMessageProcessor(ILogger<ServiceBusMessageProcessor> logger,
                                      IMessageSender<string> messageForwarder,
                                      IHostApplicationLifetime hostApplicationLifetime,
                                      ServiceBusClient serviceBusClient,
                                      TelemetryClient telemetryClient)
    {
        _logger = logger;

        _messageForwarder = messageForwarder;

        _serviceBusProcessor = serviceBusClient.CreateProcessor("sbt-azd5-dev-001", "sub-azd5-dev-001");

        _telemetryClient = telemetryClient;

        hostApplicationLifetime.ApplicationStopped.Register(async () => await StopProcessing());
    }

    public void Initialize()
    {
        _serviceBusProcessor.ProcessMessageAsync += ServiceBusMessageHandlerInParallel;

        _serviceBusProcessor.ProcessErrorAsync += ServiceBusErrorHandler;

        _telemetryClient.TrackEvent("ServiceBusMessageProcessor completed Initialization");
    }

    private async Task ServiceBusMessageHandler(ProcessMessageEventArgs args)
    {
        _telemetryClient.TrackEvent(_logger, "ServiceBusMessageProcessor received new message for processing");

        string body = args.Message.Body.ToString();

        await _messageForwarder.SendMessagesAsync([body]);

        // complete (and so delete) the message. 
        await args.CompleteMessageAsync(args.Message);
    }

    private async Task ServiceBusMessageHandlerInParallel(ProcessMessageEventArgs args)
    {
        _telemetryClient.TrackEvent(_logger, "ServiceBusMessageProcessor received new message for processing");

        string body = args.Message.Body.ToString();

        List<string> items = [];

        // create 99 versions of the item that came down in the message
        for (int i = 0; i < 99; i++)
        {
            items.Add(body);
        }

        // and start 10 threads, each uploading 99 items in batch (1 x 99 x 10 = 990)
        await Parallel.ForAsync(0, 10, async (i, state) =>
        {
            await _messageForwarder.SendMessagesAsync(items);
        });

        // complete (and so delete) the message. 
        await args.CompleteMessageAsync(args.Message);
    }

    private Task ServiceBusErrorHandler(ProcessErrorEventArgs args)
    {
        _telemetryClient.TrackException(_logger, args.Exception);

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

        _telemetryClient.TrackEvent("ServiceBusMessageProcessor stopped processing");
    }
}
