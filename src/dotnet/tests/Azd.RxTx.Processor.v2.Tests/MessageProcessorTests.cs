using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;

namespace Azd.RxTx.Processor.v2.Tests;

public class MessageProcessorTests
{
    [Fact]
    public void Constructor_Throws_On_Configuration_Not_Present()
    {
        var mockLogger = new Mock<ILogger<MessageProcessor>>();

        var mockSender = new Mock<IMessageSender<MessageBatch<string>>>();

        var mockHostApplicationLifetime = new Mock<IHostApplicationLifetime>();

        var mockConfiguration = new Mock<IConfiguration>();

        var config = TelemetryConfiguration.CreateDefault();

        var client = new TelemetryClient(config);

        Assert.Throws<ArgumentNullException>(() => new MessageProcessor(mockLogger.Object,
                                                                        mockSender.Object,
                                                                        mockHostApplicationLifetime.Object,
                                                                        mockConfiguration.Object,
                                                                        client));
    }
}