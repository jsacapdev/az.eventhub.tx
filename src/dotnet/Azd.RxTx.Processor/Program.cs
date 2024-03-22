using Microsoft.Extensions.Azure;

namespace Azd.RxTx.Processor;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<Worker>();
        builder.Services.AddApplicationInsightsTelemetryWorkerService();

        builder.Services.AddSingleton<IMessageProcessor, ServiceBusMessageProcessor>();

        builder.Services.AddSingleton<IMessageSender<string>, EventHubMessageSender>();

        builder.Services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddServiceBusClient(Environment.GetEnvironmentVariable("ServiceBusConnectionString"));

            clientBuilder.AddEventHubProducerClient(Environment.GetEnvironmentVariable("EventHubConnectionString"), 
                                                    Environment.GetEnvironmentVariable("EventHubName"));
        });

        var host = builder.Build();
        host.Run();
    }
}