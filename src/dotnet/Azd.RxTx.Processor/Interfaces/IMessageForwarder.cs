namespace Azd.RxTx.Processor;

public interface IMessageForwarder<T>
{
    void Initialize();

    Task SendMessagesAsync(IList<T> messages);
}
