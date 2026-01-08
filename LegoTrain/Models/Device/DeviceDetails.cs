// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using nanoDiscovery.Common;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace LegoTrain.Models.Device
{
    /// <summary>
    /// Represents the details of a discovered device on the network.
    /// </summary>
    public class DeviceDetails
    {
        /// <summary>
        /// Gets or sets the unique identifier for the device.
        /// </summary>
        [Display(Name = "Device's ID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the IP address of the device.
        /// </summary>
        [Display(Name = "IP Address")]
        public IPAddress IPAddress { get; set; } = IPAddress.None;

        /// <summary>
        /// Gets or sets the capabilities of the device.
        /// </summary>
        [Display(Name = "Capabilities")]
        public DeviceCapability DeviceCapacity { get; set; }

        /// <summary>
        /// Gets or sets the connection status of the device.
        /// </summary>
        [Display(Name = "Connection status")]
        public DeviceStatus DeviceStatus { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the last update received from the device.
        /// </summary>
        [Display(Name = "Latest update time")]
        public DateTimeOffset LastUpdate { get; set; }
    }
}
