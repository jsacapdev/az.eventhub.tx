using Azure.Messaging.ServiceBus;

namespace Azd.RxTx.Processor;

public class EventHubMessageForwarder : IMessageForwarder<string>
{
    private readonly ILogger<EventHubMessageForwarder> _logger;

    public EventHubMessageForwarder(ILogger<EventHubMessageForwarder> logger)
    {
        _logger = logger;
    }

    public void Initialize()
    {
        _logger.LogInformation("ServiceBusMessageProcessor completed Initialization at at: {time}", DateTimeOffset.Now);
    }

    public async Task SendMessagesAsync(IList<string> messages)
    {
        await Task.Run(() => {});
        
        throw new NotImplementedException();
    }
}
