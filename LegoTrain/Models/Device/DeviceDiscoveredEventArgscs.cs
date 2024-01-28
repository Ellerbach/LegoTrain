// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using System.Net;

namespace LegoTrain.Models.Device
{
    public class DeviceDiscoveredEventArgscs : EventArgs
    {
        public DeviceDetails DeviceDetails { get; set; }
    }
}
