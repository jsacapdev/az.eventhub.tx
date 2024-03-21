namespace Azd.RxTx.Processor;

public interface IMessageProcessor
{
    Task StartProcessingAsync();

    Task StopProcessingAsync();
}
