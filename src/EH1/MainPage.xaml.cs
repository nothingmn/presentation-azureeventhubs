using Core;
using EventHubDemo.Shared;
using System;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace EH1
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly EasClientDeviceInformation deviceInformation = new EasClientDeviceInformation();

        private readonly EventHubDefinition ehDefinition = new EventHubDefinition
        {
            ServiceNamespace = "windowsiot1",
            DeviceId = "windows10iot",
            EventHubName = "windowsiot1eventhub",
            SASGeneratedKey = "SharedAccessSignature sr=https%3a%2f%2fwindowsiot1.servicebus.windows.net%2fwindowsiot1eventhub%2fpublishers%2fwindows10iot%2fmessages&sig=1ybk5EfPez5wu3aAzI3kfpcrURt6vS7gt2pDgTnN40w%3d&se=1447125190&skn=RootManageSharedAccessKey"
        };

        private readonly Random rnd = new Random();
        private readonly ISendTelemetry telemetryProvider = new HttpTelemetryProvider();

        public MainPage()
        {
            InitializeComponent();
        }

        private async void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var result = await telemetryProvider.PostTelemetryAsync(ehDefinition, new DeviceTelemetry
            {
                DeviceId = deviceInformation.Id.ToString(),
                Type = "Simulated",
                Value = rnd.NextDouble()
            });
        }
    }
}