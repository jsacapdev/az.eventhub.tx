using System.Diagnostics;

namespace Azd.RxTx.Processor;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    private readonly IMessageProcessor _processor;

    public Worker(ILogger<Worker> logger, IMessageProcessor processor)
    {
        _logger = logger;

        _processor = processor;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _processor.Initialize();

        _logger.LogInformation("Worker started at: {time}", DateTimeOffset.Now);

        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _processor.StartProcessingAsync();

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogDebug("Worker running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(1000, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        var stopWatch = Stopwatch.StartNew();

        _logger.LogInformation("Worker stopped at: {time}", DateTimeOffset.Now);

        await base.StopAsync(cancellationToken);

        _logger.LogInformation("Worker took {ms} ms to stop.", stopWatch.ElapsedMilliseconds);
    }

}
