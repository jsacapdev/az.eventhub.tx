namespace Azd.RxTx.Processor.v2;

public interface IMessageReceiver<R>
{
    Task<R> GetMessageBatch();
}
