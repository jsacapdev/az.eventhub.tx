using System.Text;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.ApplicationInsights;

namespace Azd.RxTx.Processor;

public class EventHubMessageSender : IMessageSender<string>
{
    private readonly ILogger<EventHubMessageSender> _logger;

    private readonly EventHubProducerClient _eventHubProducerClient;

    private readonly TelemetryClient _telemetryClient;

    public EventHubMessageSender(ILogger<EventHubMessageSender> logger,
                                    IHostApplicationLifetime hostApplicationLifetime,
                                    EventHubProducerClient producerClient,
                                    TelemetryClient telemetryClient)
    {
        _logger = logger;

        _eventHubProducerClient = producerClient;

        _telemetryClient = telemetryClient;

        hostApplicationLifetime.ApplicationStopped.Register(async () => await StopSending());
    }

    public async Task SendMessagesAsync(IList<string> messages)
    {
        using EventDataBatch eventBatch = await _eventHubProducerClient.CreateBatchAsync();

        foreach (var message in messages)
        {
            if (!eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(message))))
            {
                // if it is too large for the batch
                throw new Exception($"Event {message} is too large for the batch and cannot be sent.");
            }
        }

        await _eventHubProducerClient.SendAsync(eventBatch);
    }

    private async Task StopSending()
    {
        await _eventHubProducerClient.DisposeAsync();

        _logger.LogInformation("EventHubMessageForwarder stopped sending at: {time}", DateTimeOffset.Now);
    }
}
