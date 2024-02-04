// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using nanoDiscovery.Common;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace LegoTrain.Models.Device
{
    public class DeviceDetails
    {
        [Display(Name = "Device's ID")]
        public int Id { get; set; }

        [Display(Name = "IP Address")]
        public IPAddress IPAddress { get; set; } = IPAddress.None;

        [Display(Name = "Capabilities")]
        public DeviceCapability DeviceCapacity { get; set; }

        [Display(Name = "Connection status")]
        public DeviceStatus DeviceStatus { get; set; }

        [Display(Name = "Latest update time")]
        public DateTimeOffset LastUpdate { get; set; }
    }
}
