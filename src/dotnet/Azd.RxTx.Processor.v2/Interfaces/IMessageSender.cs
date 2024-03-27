namespace Azd.RxTx.Processor.v2;

public interface IMessageSender<T>
{
    Task SendMessagesAsync(IList<T> messages);
}
