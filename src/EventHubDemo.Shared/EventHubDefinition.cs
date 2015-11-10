namespace EventHubDemo.Shared
{
    public class EventHubDefinition
    {
        public string ServiceNamespace { get; set; }
        public string EventHubName { get; set; }
        public string DeviceId { get; set; }
        public string ConnectionString { get; set; }

        // https://blogs.endjin.com/2015/02/send-data-into-azure-event-hubs-using-web-apis-httpclient/
        // https://github.com/sandrinodimattia/RedDog/releases/tag/0.2.0.1
        // http://fabriccontroller.net/blog/posts/iot-with-azure-service-bus-event-hubs-authenticating-and-sending-from-any-type-of-device-net-and-js-samples/
        public string SASGeneratedKey { get; set; }

        public string StorageAccountName { get; set; }
        public string StorageAccountKey { get; set; }
    }
}