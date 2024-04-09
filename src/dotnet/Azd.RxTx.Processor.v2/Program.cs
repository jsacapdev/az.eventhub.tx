using Azure.Identity;
using Microsoft.ApplicationInsights.Extensibility;
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
            options.ServiceName = "RxTx v2 Service";
        });

        builder.Services.AddSerilog(config =>
        {
            config.ReadFrom.Configuration(builder.Configuration);
        });

        builder.Services.AddHostedService<Worker>();
        builder.Services.AddApplicationInsightsTelemetryWorkerService();
        builder.Services.AddSingleton<ITelemetryInitializer, AzdRxTxProcessorTelemetryInitializer>();

        builder.Services.AddSingleton<IMessageReceiver<MessageBatch<string>>, MessageReceiver>();
        builder.Services.AddSingleton<IMessageProcessor<MessageBatch<string>>, MessageProcessor>();
        builder.Services.AddSingleton<IMessageSender<MessageBatch<string>>, EventHubMessageSender>();

        if (builder.Environment.IsProduction())
        {
            builder.Configuration.AddAzureKeyVault(new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
                new DefaultAzureCredential());
        }

        builder.Services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddEventHubProducerClientWithNamespace(builder.Configuration["EventHub:Namespace"],
                                                                 builder.Configuration["EventHub:Name"]);

            clientBuilder.UseCredential(new DefaultAzureCredential());
        });

        var host = builder.Build();
        host.Run();
    }
}