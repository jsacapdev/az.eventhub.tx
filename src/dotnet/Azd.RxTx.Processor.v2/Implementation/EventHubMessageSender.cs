using System.Text;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.ApplicationInsights;

namespace Azd.RxTx.Processor.v2;

public class EventHubMessageSender : IMessageSender<MessageBatch<string>>
{
    private readonly ILogger<EventHubMessageSender> _logger;

    private readonly EventHubProducerClient _eventHubProducerClient;

    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    private readonly TelemetryClient _telemetryClient;

    public EventHubMessageSender(ILogger<EventHubMessageSender> logger,
                                IHostApplicationLifetime hostApplicationLifetime,
                                EventHubProducerClient producerClient,
                                TelemetryClient telemetryClient)
    {
        _logger = logger;

        _eventHubProducerClient = producerClient;

        _hostApplicationLifetime = hostApplicationLifetime;

        _hostApplicationLifetime.ApplicationStopped.Register(async () => await StopSending());

        _telemetryClient = telemetryClient;
    }

    public async Task SendBatchAsync(MessageBatch<string> batch)
    {
        using EventDataBatch eventBatch = await _eventHubProducerClient.CreateBatchAsync();

        foreach (var message in batch.Items)
        {
            if (!eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(message))))
            {
                // if it is too large for the batch
                throw new Exception($"Event {message} is too large for the batch and cannot be sent.");
            }
        }

        try
        {
            await _eventHubProducerClient.SendAsync(eventBatch);
        }
        catch (Exception ex)
        {
            _telemetryClient.TrackException(_logger, ex);

            _hostApplicationLifetime.StopApplication();
        }
    }

    private async Task StopSending()
    {
        await _eventHubProducerClient.DisposeAsync();

        _logger.LogInformation($"EventHubMessageSender stopped processing at: {DateTimeOffset.Now}");
    }
}
