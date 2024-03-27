using Microsoft.Extensions.Azure;
using Serilog;

namespace Azd.RxTx.Processor.v2;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddWindowsService(options =>
        {
            options.ServiceName = "RxTx Service";
        });

        builder.Services.AddSerilog(config =>
        {
            config.ReadFrom.Configuration(builder.Configuration);
        });

        builder.Services.AddHostedService<Worker>();
        builder.Services.AddApplicationInsightsTelemetryWorkerService();

        builder.Services.AddSingleton<IMessageReceiver<MessageBatch<string>>, MessageReceiver>();
        builder.Services.AddSingleton<IMessageProcessor<MessageBatch<string>>, MessageProcessor>();
        builder.Services.AddSingleton<IMessageSender<MessageBatch<string>>, EventHubMessageSender>();

        builder.Services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddEventHubProducerClient(Environment.GetEnvironmentVariable("EventHubConnectionString"),
                                                    Environment.GetEnvironmentVariable("EventHubName"));
        });

        var host = builder.Build();
        host.Run();
    }
}