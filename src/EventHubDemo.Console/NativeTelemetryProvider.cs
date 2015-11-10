using EventHubDemo.Shared;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace EventHubDemo.Console
{
    public class NativeTelemetryProvider : ISendTelemetry
    {
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

                //evtData.PartitionKey     ///set only if you care about ordering

                await _eventHub.SendAsync(evtData);
                return true;
            }
            catch (Exception e)
            {
                //log your exception somehow...
                return false;
            }
        }
    }
}