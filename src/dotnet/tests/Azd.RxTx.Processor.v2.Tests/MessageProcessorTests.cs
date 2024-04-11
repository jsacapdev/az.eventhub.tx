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

    [Fact]
    public void Can_Process_Message_Single_Thread_OK()
    {
        var reset = new AutoResetEvent(false);
        var mockLogger = new Mock<ILogger<MessageProcessor>>();
        var mockHostApplicationLifetime = new Mock<IHostApplicationLifetime>();
        var config = TelemetryConfiguration.CreateDefault();
        var client = new TelemetryClient(config);

        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration.SetupGet(x => x[It.Is<string>(s => s == "MessageProcessorThreadCount")]).Returns("1");

        var mockSender = new Mock<IMessageSender<MessageBatch<string>>>();
        mockSender.Setup(sender => sender.SendBatchAsync(It.IsAny<MessageBatch<string>>()))
                  .Callback(() =>
                  {
                      reset.Set();
                  });

        var processor = new MessageProcessor(mockLogger.Object,
                                             mockSender.Object,
                                             mockHostApplicationLifetime.Object,
                                             mockConfiguration.Object,
                                             client);
        processor.Initialize();

        processor.Enqueue(new(["mockData"]));

        var wasSignaled = reset.WaitOne(timeout: TimeSpan.FromSeconds(60));

        mockSender.Verify(sender => sender.SendBatchAsync(It.IsAny<MessageBatch<string>>()), Times.AtMostOnce());
    }
}