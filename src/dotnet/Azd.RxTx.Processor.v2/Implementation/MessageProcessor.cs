using System.Collections.Concurrent;
using Microsoft.ApplicationInsights;

namespace Azd.RxTx.Processor.v2;

public class MessageProcessor : IMessageProcessor<MessageBatch<string>>
{
    private readonly ILogger<MessageProcessor> _logger;

    private readonly IMessageSender<MessageBatch<string>> _sender;

    private readonly TelemetryClient _telemetryClient;

    private readonly BlockingCollection<MessageBatch<string>> _events = [];

    public MessageProcessor(ILogger<MessageProcessor> logger,
                            IMessageSender<MessageBatch<string>> sender,
                            IHostApplicationLifetime hostApplicationLifetime,
                            TelemetryClient telemetryClient)
    {
        _logger = logger;

        _sender = sender;

        _telemetryClient = telemetryClient;

        hostApplicationLifetime.ApplicationStopped.Register(async () => await StopProcessing());
    }

    public void Initialize()
    {
        for (int i = 0; i < 5; i++)
        {
            Task.Factory.StartNew(ProcessQueueAsync, TaskCreationOptions.LongRunning);
        }

        _logger.LogInformation($"Initialized MessageProcessor at: {DateTimeOffset.Now}");
    }

    public void Enqueue(MessageBatch<string> eventDataBatch)
    {
        _events.Add(eventDataBatch);
    }

    private async Task ProcessQueueAsync()
    {
        foreach (var item in _events.GetConsumingEnumerable())
        {
            await ProcessItemAsync(item);
        }
    }

    private async Task ProcessItemAsync(MessageBatch<string> eventBatch)
    {
        await _sender.SendBatchAsync(eventBatch);

        _telemetryClient.TrackTrace(_logger, $"Completed processing item {eventBatch.Id} at: {DateTimeOffset.Now}");
    }

    private async Task StopProcessing()
    {
        await Task.Run(() => { });

        _events.CompleteAdding();

        _logger.LogInformation($"MessageProcessor stopped processing at: {DateTimeOffset.Now}");
    }
}
