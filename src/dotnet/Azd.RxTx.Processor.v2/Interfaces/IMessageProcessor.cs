namespace Azd.RxTx.Processor.v2;

public interface IMessageProcessor<T>
{
    void Initialize();

    void Enqueue(T item);
}
