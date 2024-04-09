using System.Collections.Concurrent;
using Microsoft.ApplicationInsights;

namespace Azd.RxTx.Processor.v2;

public class MessageProcessor : IMessageProcessor<MessageBatch<string>>
{
    private readonly ILogger<MessageProcessor> _logger;

    private readonly IMessageSender<MessageBatch<string>> _sender;

    private readonly TelemetryClient _telemetryClient;

    private readonly BlockingCollection<MessageBatch<string>> _events = [];

    private readonly int _threadCount;

    public MessageProcessor(ILogger<MessageProcessor> logger,
                            IMessageSender<MessageBatch<string>> sender,
                            IHostApplicationLifetime hostApplicationLifetime,
                            IConfiguration configuration,
                            TelemetryClient telemetryClient)
    {
        _logger = logger;

        _sender = sender;

        _telemetryClient = telemetryClient;

        if (!int.TryParse(configuration["MessageProcessorThreadCount"], out _threadCount))
        {
            throw new ArgumentNullException("MessageProcessorThreadCount is not set in configuration.");
        }

        hostApplicationLifetime.ApplicationStopped.Register(() => StopProcessing());
    }

    public void Initialize()
    {
        for (int i = 0; i < _threadCount; i++)
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

    private void StopProcessing()
    {
        _events.CompleteAdding();

        _logger.LogInformation($"MessageProcessor stopped processing at: {DateTimeOffset.Now}");
    }
}
