using EventHubDemo.Shared;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class HttpTelemetryProvider : ISendTelemetry
    {
        public async Task<bool> PostTelemetryAsync(EventHubDefinition eventHub, DeviceTelemetry deviceTelemetry)
        {
            // Namespace info.
            var url = new Uri(string.Format("https://{0}.servicebus.windows.net/{1}/publishers/{2}/messages", eventHub.ServiceNamespace, eventHub.EventHubName, eventHub.DeviceId));
            // Create client.
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Origin", "*");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Access-Control-Allow-Credentials", "true");
            var payload = JsonConvert.SerializeObject(deviceTelemetry);

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", eventHub.SASGeneratedKey);

            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            content.Headers.Add("ContentType", "application/json");

            var result = await httpClient.PostAsync(url, content);
            return result.IsSuccessStatusCode;
        }
    }
}