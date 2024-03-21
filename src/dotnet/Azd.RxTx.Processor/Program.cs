using Microsoft.Extensions.Azure;

namespace Azd.RxTx.Processor;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<Worker>();

        builder.Services.AddSingleton<IMessageProcessor, ServiceBusMessageProcessor>();

        foreach (var def in Environment.GetEnvironmentVariables())
        { 
            Console.WriteLine(def);

        }

        builder.Services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddServiceBusClient(Environment.GetEnvironmentVariable("ServiceBusConnectionString"));
        });

        var host = builder.Build();
        host.Run();
    }
}