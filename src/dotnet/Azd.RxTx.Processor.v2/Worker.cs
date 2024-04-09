using System.Diagnostics;
using Azure.Messaging.EventHubs.Producer;

namespace Azd.RxTx.Processor.v2;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    private readonly IMessageProcessor<MessageBatch<string>> _processor;

    private readonly IMessageReceiver<MessageBatch<string>> _receiver;

    public Worker(ILogger<Worker> logger, 
                  IMessageReceiver<MessageBatch<string>> receiver, 
                  IMessageProcessor<MessageBatch<string>> processor)
    {
        _logger = logger;

        _receiver = receiver;

        _processor = processor;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _processor.Initialize();

        _logger.LogInformation($"Worker started at: {DateTimeOffset.Now}");

        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogDebug("Worker running at: {time}", DateTimeOffset.Now);
            }

            var batch = await _receiver.GetMessageBatch();

            _processor.Enqueue(batch);

            await Task.Delay(1000, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        var stopWatch = Stopwatch.StartNew();

        _logger.LogInformation($"Worker stopped at: {DateTimeOffset.Now}");

        await base.StopAsync(cancellationToken);

        _logger.LogInformation($"Worker took {stopWatch.ElapsedMilliseconds} ms to stop.");
    }
}
