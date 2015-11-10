using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHubDemo.Subscriber.Console
{
    public class FarBetterEventProcessor : IEventProcessor
    {
        private Stopwatch checkpointStopWatch;

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
            checkpointStopWatch = new Stopwatch();
            checkpointStopWatch.Start();
            return Task.FromResult<object>(null);
        }

        async Task IEventProcessor.ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            var partitionedMessages = messages.GroupBy(data => data.PartitionKey).ToDictionary(datas => datas.Key, datas => datas.ToList());

            //For each partition spawn a Task which will sequentially iterate over its own block
            //Wait for all Tasks to complete, before proceeding.
            await Task.WhenAll(partitionedMessages.Select(partition => Task.Run(async () =>
            {
                var block = partition.Value;
                foreach (var eventData in block)
                {
                    try
                    {
                        var data = Encoding.UTF8.GetString(eventData.GetBytes());

                        System.Console.WriteLine(DateTime.Now + ":Message received.  Partition: '{0}', Data: '{1}', Partition Key: '{2}'", context.Lease.PartitionId, data, eventData.PartitionKey);
                    }
                    catch (Exception e)
                    {
                        //do something with your logs..
                    }
                }
            })));

            //Call checkpoint every 5 minutes, so that worker can resume processing from the 5 minutes back if it restarts.
            if (checkpointStopWatch.Elapsed > TimeSpan.FromMinutes(5))
            {
                await context.CheckpointAsync();
                checkpointStopWatch.Restart();
            }
        }
    }
}