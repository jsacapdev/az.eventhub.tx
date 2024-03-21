namespace Azd.RxTx.Processor;

public interface IMessageSender<T>
{
    Task SendMessagesAsync(IList<T> messages);
}
