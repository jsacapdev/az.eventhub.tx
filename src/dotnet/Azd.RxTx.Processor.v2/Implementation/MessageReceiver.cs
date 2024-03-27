using Newtonsoft.Json;

namespace Azd.RxTx.Processor.v2;

public class MessageReceiver : IMessageReceiver<MessageBatch<string>>
{
    private readonly ILogger<MessageReceiver> _logger;

    public MessageReceiver(ILogger<MessageReceiver> logger,
                           IHostApplicationLifetime hostApplicationLifetime)
    {
        _logger = logger;

        hostApplicationLifetime.ApplicationStopped.Register(() => StopReceiving());
    }

    public async Task<MessageBatch<string>> GetMessageBatch()
    {
        await Task.Run(() => {});

        List<string> messages = [];

        for (int i = 0; i < 100; i++)
        {
            messages.Add(JsonConvert.SerializeObject(new {Id = Guid.NewGuid().ToString(), Date = DateTimeOffset.UtcNow}));
        }

        return new MessageBatch<string>(messages);
    }

    private void StopReceiving()
    {
        _logger.LogInformation($"MessageReceiver stopped receiving at: {DateTimeOffset.Now}");
    }
}
