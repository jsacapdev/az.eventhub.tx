namespace Azd.RxTx.Processor;

public interface IMessageProcessor
{
    void Initialize();

    Task StartProcessingAsync();
}
