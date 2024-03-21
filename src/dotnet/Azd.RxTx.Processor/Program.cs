namespace Azd.RxTx.Processor;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<Worker>();

        builder.Services.AddSingleton<IMessageProcessor, ServiceBusMessageProcessor>();

        var host = builder.Build();
        host.Run();
    }
}