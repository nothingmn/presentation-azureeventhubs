using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventHubDemo.Subscriber.Console
{
    public class BadEventProcessor : IEventProcessor
    {
        async Task IEventProcessor.CloseAsync(PartitionContext context, CloseReason reason)
        {
            System.Console.WriteLine(DateTime.Now + ":Processor Shutting Down. Partition '{0}', Reason: '{1}'.", context.Lease.PartitionId, reason);
            if (reason == CloseReason.Shutdown)
            {
                await context.CheckpointAsync();
            }
        }

        Task IEventProcessor.OpenAsync(PartitionContext context)
        {
            System.Console.WriteLine(DateTime.Now + ":SimpleEventProcessor initialized.  Partition: '{0}', Offset: '{1}'", context.Lease.PartitionId, context.Lease.Offset);
            return Task.FromResult<object>(null);
        }

        async Task IEventProcessor.ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            //The problem with this is that we are not guaranteeing each item in the Parallel loop is getting processed in order, by PartitionKey
            Parallel.ForEach(messages, eventData =>
            {
                var data = Encoding.UTF8.GetString(eventData.GetBytes());

                System.Console.WriteLine(DateTime.Now + ":Message received.  Partition: '{0}', Data: '{1}', Partition Key: '{2}'", context.Lease.PartitionId, data, eventData.PartitionKey);
            });

            //No checkpointing?  You are not going to have a good time.
            //await context.CheckpointAsync();
        }
    }
}