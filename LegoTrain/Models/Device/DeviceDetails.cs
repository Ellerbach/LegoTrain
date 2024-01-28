// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using nanoDiscovery.Common;
using System.Net;

namespace LegoTrain.Models.Device
{
    public class DeviceDetails
    {
        public int Id { get; set; }

        public IPAddress IPAddress { get; set; } = IPAddress.None;

        public DeviceCapability DeviceCapacity { get; set; }

        public DeviceStatus DeviceStatus { get; set; }

        public DateTimeOffset LastUpdate { get; set; }
    }
}
