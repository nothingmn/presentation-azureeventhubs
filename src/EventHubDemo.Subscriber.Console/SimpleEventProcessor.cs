﻿using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventHubDemo.Subscriber.Console
{
    public class SimpleEventProcessor : IEventProcessor
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
            foreach (var eventData in messages)
            {
                var data = Encoding.UTF8.GetString(eventData.GetBytes());

                System.Console.WriteLine(DateTime.Now + ":Message received.  Partition: '{0}', Data: '{1}', Partition Key: '{2}'", context.Lease.PartitionId, data, eventData.PartitionKey);
            }

            //Call checkpoint on every receive..
            //Generally considered a bad idea.
            //There is significant overhead by doing this.
            await context.CheckpointAsync();
        }
    }
}