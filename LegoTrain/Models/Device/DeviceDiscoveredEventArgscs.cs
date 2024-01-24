using System.Net;

namespace LegoTrain.Models.Device
{
    public class DeviceDiscoveredEventArgscs : EventArgs
    {
        public DeviceDetails DeviceDetails { get; set; }
    }
}
