using System.Threading.Tasks;

namespace EventHubDemo.Shared
{
    public interface ISendTelemetry
    {
        Task<bool> PostTelemetryAsync(EventHubDefinition eventHub, DeviceTelemetry deviceTelemetry);
    }
}