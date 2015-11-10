using EventHubDemo.Shared;
using Microsoft.ServiceBus.Messaging;
using System;

namespace EventHubDemo.Subscriber.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var ehDefinition = new EventHubDefinition
            {
                ServiceNamespace = "windowsiot1",
                DeviceId = "windows10iot",
                EventHubName = "windowsiot1eventhub",
                ConnectionString = "Endpoint=sb://windowsiot1.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=gyj5SAwTzrVE3f1XFuT4uyC7bqB82hcPu61D/wx7kl8=",
                StorageAccountKey = "FyOzVoluW3xGYWWslUdwYk9yPYOi5C+K69bDDQpGIvZ4SwFRXfuFtd0I/sJCxC0k26GLWQK1OY+BSiyAdFzbww==",
                StorageAccountName = "ehstorage1"
            };

            var storageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", ehDefinition.StorageAccountName, ehDefinition.StorageAccountKey);

            var eventProcessorHostName = Guid.NewGuid().ToString();
            var eventProcessorHost = new EventProcessorHost(eventProcessorHostName, ehDefinition.EventHubName, EventHubConsumerGroup.DefaultGroupName, ehDefinition.ConnectionString, storageConnectionString);

            System.Console.WriteLine(DateTime.Now + ":Registering EventProcessor...");
            eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>().Wait();

            System.Console.WriteLine(DateTime.Now + ":Receiving. Press enter key to stop worker.");
            System.Console.ReadLine();
            eventProcessorHost.UnregisterEventProcessorAsync().Wait();
        }
    }
}