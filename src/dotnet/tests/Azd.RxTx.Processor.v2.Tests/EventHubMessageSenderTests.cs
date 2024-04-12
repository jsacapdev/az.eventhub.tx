using System.Text;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;

namespace Azd.RxTx.Processor.v2.Tests;

public class EventHubMessageSenderTests
{
    [Fact]
    public async Task Event_Hub_Message_Sender_Uploads_Batch_Successfully()
    {
        var mockLogger = new Mock<ILogger<EventHubMessageSender>>();
        var mockEventHubProducerClient = new Mock<EventHubProducerClient>();
        var mockHostApplicationLifetime = new Mock<IHostApplicationLifetime>();
        var config = TelemetryConfiguration.CreateDefault();
        var client = new TelemetryClient(config);

        int batchCountThreshold = 3;
        long batchSizeInBytes = 500;

        // As events are added to the batch they will be added to this list as well. 
        List<EventData> backingList = [];

        EventDataBatch dataBatchMock = EventHubsModelFactory.EventDataBatch(
                batchSizeBytes: batchSizeInBytes,
                batchEventStore: backingList,
                batchOptions: new CreateBatchOptions(),
                eventData =>
                {
                    int eventCount = backingList.Count;
                    return eventCount < batchCountThreshold;
                });


        List<string> sourceEvents = [];

        for (int index = 0; index < batchCountThreshold; index++)
        {
            sourceEvents.Add($"Sample-Event-{index}");
        }

        MessageBatch<string> batch = new MessageBatch<string>(sourceEvents);

        // This sets up a mock of the CreateBatchAsync method, returning the batch that was previously mocked.
        mockEventHubProducerClient.Setup(p => p.CreateBatchAsync(It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(dataBatchMock);

        // SendAsync method so it will throw an exception if the batch passed into it is not the one we are expecting to send.
        mockEventHubProducerClient.Setup(p => p.SendAsync(It.Is<EventDataBatch>(sendBatch => sendBatch != dataBatchMock), It.IsAny<CancellationToken>()))
                                  .Throws(new Exception("The batch published was not the expected batch."));

        var sender = new EventHubMessageSender(mockLogger.Object,
                                               mockHostApplicationLifetime.Object,
                                               mockEventHubProducerClient.Object,
                                               client);

        await sender.SendBatchAsync(batch);

        // check that the events in the batch match what the application expects to have added.
        foreach (EventData eventData in backingList)
        {
            var data = Encoding.UTF8.GetString(eventData.EventBody);

            Assert.Contains(data, sourceEvents);
        }

        Assert.Equal(backingList.Count, sourceEvents.Count);

        // ensure that an instruction to shut down the service was not received (critical failure)
        mockHostApplicationLifetime.Verify(host => host.StopApplication(), Times.Never());

        mockEventHubProducerClient.Verify(producer => producer.CreateBatchAsync(It.IsAny<CancellationToken>()), Times.Once());
        // Verify SendAsync was called once within the mocked producer client
        mockEventHubProducerClient.Verify(producer => producer.SendAsync(It.IsAny<EventDataBatch>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task Event_Hub_Message_Sender_Stops_Service_On_Critical_Failure()
    {
        var mockLogger = new Mock<ILogger<EventHubMessageSender>>();
        var mockEventHubProducerClient = new Mock<EventHubProducerClient>();
        var mockHostApplicationLifetime = new Mock<IHostApplicationLifetime>();
        var config = TelemetryConfiguration.CreateDefault();
        var client = new TelemetryClient(config);

        int batchCountThreshold = 3;
        long batchSizeInBytes = 500;
        List<EventData> backingList = [];

        EventDataBatch dataBatchMock = EventHubsModelFactory.EventDataBatch(
                batchSizeBytes: batchSizeInBytes,
                batchEventStore: backingList,
                batchOptions: new CreateBatchOptions(),
                eventData =>
                {
                    int eventCount = backingList.Count;
                    return eventCount < batchCountThreshold;
                });


        MessageBatch<string> batch = new MessageBatch<string>([]);

        mockEventHubProducerClient.Setup(p => p.CreateBatchAsync(It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(dataBatchMock);

        // SendAsync method throws an exception which means the service cannot send anything to event hub, so stop the service
        mockEventHubProducerClient.Setup(p => p.SendAsync(It.IsAny<EventDataBatch>(), It.IsAny<CancellationToken>()))
                                  .Throws(new Exception("The event hub is not available."));

        var sender = new EventHubMessageSender(mockLogger.Object,
                                               mockHostApplicationLifetime.Object,
                                               mockEventHubProducerClient.Object,
                                               client);

        await sender.SendBatchAsync(batch);

        // ensure the instruction to shut down the worker was received
        mockHostApplicationLifetime.Verify(host => host.StopApplication(), Times.Once());

        mockEventHubProducerClient.Verify(producer => producer.CreateBatchAsync(It.IsAny<CancellationToken>()), Times.Once());
        mockEventHubProducerClient.Verify(producer => producer.SendAsync(It.IsAny<EventDataBatch>(), It.IsAny<CancellationToken>()), Times.Once());
    }
}