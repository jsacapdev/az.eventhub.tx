using System.Text;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

namespace Azd.RxTx.Processor.v2;

public class MessageReceiver : IMessageReceiver<EventDataBatch>
{
    private readonly ILogger<MessageReceiver> _logger;

    private readonly EventHubProducerClient _eventHubProducerClient;

    public MessageReceiver(ILogger<MessageReceiver> logger,
                           IHostApplicationLifetime hostApplicationLifetime,
                           EventHubProducerClient producerClient)
    {
        _logger = logger;

        _eventHubProducerClient = producerClient;

        hostApplicationLifetime.ApplicationStopped.Register(() => StopReceiving());
    }

    public async Task<EventDataBatch> GetMessageBatch()
    {
        using EventDataBatch eventBatch = await _eventHubProducerClient.CreateBatchAsync();

        List<string> messages = [];

        // create 99 versions of the item that came down in the message
        for (int i = 0; i < 99; i++)
        {
            messages.Add("body");
        }

        foreach (var message in messages)
        {
            if (!eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(message))))
            {
                // if it is too large for the batch
                throw new Exception($"Event {message} is too large for the batch and cannot be sent.");
            }
        }

        return eventBatch;
    }

    private void StopReceiving()
    {
        _logger.LogInformation($"MessageReceiver stopped receiving at: {DateTimeOffset.Now}");
    }
}
