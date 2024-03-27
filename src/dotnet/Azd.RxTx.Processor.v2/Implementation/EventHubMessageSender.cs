using System.Text;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

namespace Azd.RxTx.Processor.v2;

public class EventHubMessageSender : IMessageSender<MessageBatch<string>>
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

        await _eventHubProducerClient.SendAsync(eventBatch);
    }

    private async Task StopSending()
    {
        await _eventHubProducerClient.DisposeAsync();

        _logger.LogInformation($"EventHubMessageSender stopped processing at: {DateTimeOffset.Now}");
    }
}
