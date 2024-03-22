using Azure.Messaging.ServiceBus;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

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
        _serviceBusProcessor.ProcessMessageAsync += ServiceBusMessageHandler;

        _serviceBusProcessor.ProcessErrorAsync += ServiceBusErrorHandler;

        _logger.LogInformation("ServiceBusMessageProcessor completed Initialization at: {time}", DateTimeOffset.Now);
    }

    private async Task ServiceBusMessageHandler(ProcessMessageEventArgs args)
    {
        _telemetryClient.TrackEvent(_logger, "ServiceBusMessageProcessor received new message for processing");

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