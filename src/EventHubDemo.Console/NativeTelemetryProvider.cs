using EventHubDemo.Shared;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EventHubDemo.Console
{
    public class NativeTelemetryProvider : ISendTelemetry
    {
        public static int PartitionCount { get; set; } = 4;

        private readonly EventHubDefinition _definition;
        private readonly EventHubClient _eventHub;

        public NativeTelemetryProvider(EventHubDefinition definition)
        {
            _definition = definition;
            _eventHub = EventHubClient.CreateFromConnectionString(_definition.ConnectionString, _definition.EventHubName);
        }

        public async Task<bool> PostTelemetryAsync(EventHubDefinition eventHub, DeviceTelemetry deviceTelemetry)
        {
            try
            {
                var data = JsonConvert.SerializeObject(deviceTelemetry);
                var evtData = new EventData(Encoding.UTF8.GetBytes(data));

                ///set only if you care about sequential delivery
                evtData.PartitionKey = ComputeBucket(Guid.NewGuid().ToString(), PartitionCount);

                await _eventHub.SendAsync(evtData);
                return true;
            }
            catch (Exception e)
            {
                //log your exception somehow...
                return false;
            }
        }

        /// <summary>
        /// A sample method to bucketize any string input into X number of buckets
        /// </summary>
        /// <param name="id"></param>
        /// <param name="partitionCount"></param>
        /// <returns></returns>
        private string ComputeBucket(string id, int bucketCount)
        {
            using (var sha = new SHA256Managed())
            {
                var x = sha.ComputeHash(Encoding.UTF8.GetBytes(id));
                var y = BitConverter.ToInt32(x, 0) + 0xDEADBEEF;
                return ((int)(y % bucketCount)).ToString();
            }
        }
    }
}