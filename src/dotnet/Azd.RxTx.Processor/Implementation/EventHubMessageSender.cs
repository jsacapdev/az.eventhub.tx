using System.Text;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

namespace Azd.RxTx.Processor;

public class EventHubMessageSender : IMessageSender<string>
{
    private readonly ILogger<EventHubMessageSender> _logger;

    private readonly EventHubProducerClient _eventHubProducerClient;

    public EventHubMessageSender(ILogger<EventHubMessageSender> logger,
                                    IHostApplicationLifetime hostApplicationLifetime,
                                    EventHubProducerClient producerClient)
    {
        _logger = logger;

        _eventHubProducerClient = producerClient;

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

        _logger.LogInformation("EventHubMessageForwarder completed sending a batch of {count} events at: {time}", 
                               messages.Count, 
                               DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", System.Globalization.CultureInfo.InvariantCulture));
    }

    private async Task StopSending()
    {
        await _eventHubProducerClient.DisposeAsync();

        _logger.LogInformation("EventHubMessageForwarder stopped sending at: {time}", DateTimeOffset.Now);
    }
}
