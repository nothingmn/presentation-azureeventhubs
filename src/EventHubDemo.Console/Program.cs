using EventHubDemo.Shared;
using System;
using System.Threading.Tasks;

namespace EventHubDemo.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            var p = new Program();
            p.MainAsync(args).Wait();
        }

        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
        }

        private async Task MainAsync(string[] args)
        {
            var ehDefinition = new EventHubDefinition
            {
                ServiceNamespace = "windowsiot1",
                DeviceId = "windows10iot",
                EventHubName = "windowsiot1eventhub",
                ConnectionString = "Endpoint=sb://windowsiot1.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=gyj5SAwTzrVE3f1XFuT4uyC7bqB82hcPu61D/wx7kl8="
            };

            var telemetryProvider = new NativeTelemetryProvider(ehDefinition);

            var tempProvider = new SystemTemperature();
            System.Console.Write("Ready, hit q to quit >");
            var keepGoing = System.Console.ReadKey().Key != ConsoleKey.Q;
            var order = 0;
            while (keepGoing)
            {
                var result = await telemetryProvider.PostTelemetryAsync(ehDefinition, new DeviceTelemetry
                {
                    DeviceId = ehDefinition.DeviceId,
                    Type = "Temperature",
                    Value = tempProvider.GetTemperature(),
                    Order = ++order
                });
                System.Console.WriteLine(DateTime.Now + ":Sent some data");
                //keepGoing = System.Console.ReadKey().Key != ConsoleKey.Q;
            }
        }
    }
}