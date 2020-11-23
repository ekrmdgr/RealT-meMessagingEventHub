using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Messaging.EventHubs.Producer;
using Azure.Storage.Blobs;
using RealTimeMessaging.Orchestrator.Interfaces;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RealTimeMessaging.Orchestrator.MessagingQueues
{
    public class EventHubQueue : IEventHubQueue
    {

        private Action<string> _eventHandler { get; set; }
        private Action<string> _errorHandler { get; set; }

        public EventHubQueue()
        {
            
        }


        public async void Listen(Action<string> eventHandler, Action<string> errorHandler)
        {
            _eventHandler = eventHandler;
            _errorHandler = errorHandler;

            // Read from the default consumer group: $Default
            string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

            // Create a blob container client that the event processor will use 
            BlobContainerClient storageClient = new BlobContainerClient("DefaultEndpointsProtocol=https;AccountName=testblobstorage2021;AccountKey=hN77k/JHnRt+lGysIHJHuP2U9dvbIrYkPuwWrBN0O7ouj6mbQ47syeBxzmjvnPNugzVCXVancO8mVMXL8E0G8Q==;EndpointSuffix=core.windows.net", "blobcontainer");

            // Create an event processor client to process events in the event hub
            EventProcessorClient processor = new EventProcessorClient(storageClient, consumerGroup, "Endpoint=sb://testeventhub2021.servicebus.windows.net/;SharedAccessKeyName=eventhub;SharedAccessKey=dpDp+H+RKbIkqsLhzYmqlCSzJY9SI/WNm/e3vFk+1Ys=;EntityPath=eventhubdemo", "eventhubdemo");

            // Register handlers for processing events and handling errors
            processor.ProcessEventAsync += ProcessEventHandler;
            processor.ProcessErrorAsync += ProcessErrorHandler;

            // Start the processing
            await processor.StartProcessingAsync();

            // Wait for 10 seconds for the events to be processed
            await Task.Delay(Timeout.InfiniteTimeSpan);

            // Stop the processing
            await processor.StopProcessingAsync();
        }

        private Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
        {
            // Write details about the error to the console window
            Console.WriteLine($"\tPartition '{ eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
            Console.WriteLine(eventArgs.Exception.Message);

            _errorHandler(eventArgs.Exception.Message);

            return Task.CompletedTask;
        }

        private async Task ProcessEventHandler(ProcessEventArgs arg)
        {
            // Write the body of the event to the console window
            Console.WriteLine("\tRecevied event: {0}", Encoding.UTF8.GetString(arg.Data.Body.ToArray()));

            _eventHandler(Encoding.UTF8.GetString(arg.Data.Body.ToArray()));

            // Update checkpoint in the blob storage so that the app receives only new events the next time it's run
            await arg.UpdateCheckpointAsync(arg.CancellationToken);
        }

        public async void Send(IMessage message)
        {
            await using (var producerClient = new EventHubProducerClient("Endpoint=sb://testeventhub2021.servicebus.windows.net/;SharedAccessKeyName=eventhub;SharedAccessKey=dpDp+H+RKbIkqsLhzYmqlCSzJY9SI/WNm/e3vFk+1Ys=;EntityPath=eventhubdemo", "eventhubdemo"))
            {
                // Create a batch of events 
                using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

                // Add events to the batch. An event is a represented by a collection of bytes and metadata. 
                eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(message.Body)));

                // Use the producer client to send the batch of events to the event hub
                await producerClient.SendAsync(eventBatch);
            }
        }
    }
}
