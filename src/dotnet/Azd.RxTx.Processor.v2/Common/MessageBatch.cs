namespace Azd.RxTx.Processor.v2;

public class MessageBatch<T>
{
    public MessageBatch(List<T> items)
    {
        Items = items;

        Id = Guid.NewGuid().ToString();
    }

    public string Id  { get; private set; }

    public List<T> Items { get; private set; }
}
