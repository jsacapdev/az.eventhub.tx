using System.Collections.Concurrent;
using Azure.Messaging.EventHubs.Producer;

namespace Azd.RxTx.Processor.v2;

public class MessageProcessor : IMessageProcessor<EventDataBatch>
{
    private readonly ILogger<MessageProcessor> _logger;

    private readonly BlockingCollection<EventDataBatch> _events = [];

    public MessageProcessor(ILogger<MessageProcessor> logger,
                           IHostApplicationLifetime hostApplicationLifetime)
    {
        _logger = logger;

        hostApplicationLifetime.ApplicationStopped.Register(async () => await StopProcessing());
    }

    public void Initialize()
    {
        for (int i = 0; i < 3; i++)
        {
            var thread = new Thread(ProcessQueue)
            {
                // This is important as it allows the process to exit while this thread is running
                IsBackground = true
            };
            thread.Start();
        }

        _logger.LogInformation($"Initialized MessageProcessor at: {DateTimeOffset.Now}");
    }

    public void Enqueue(EventDataBatch eventDataBatch)
    {
        _events.Add(eventDataBatch);
    }

    private void ProcessQueue()
    {
        foreach (var item in _events.GetConsumingEnumerable())
        {
            ProcessItem(item);
        }
    }

    private void ProcessItem(EventDataBatch eventBatch)
    {
        _logger.LogInformation($"Processing item at: {DateTimeOffset.Now}");
    }

    private async Task StopProcessing()
    {
        await Task.Run(() => { });

        _events.CompleteAdding();

        _logger.LogInformation($"MessageProcessor stopped processing at: {DateTimeOffset.Now}");
    }
}
