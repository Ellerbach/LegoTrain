// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;

namespace LegoTrain.Models
{
    /// <summary>
    /// Represents an infrared transmitter device for controlling Lego trains.
    /// </summary>
    public class Infrared
    {
        /// <summary>
        /// Gets or sets the IP address of the infrared module.
        /// </summary>
        [Display(Name = "IP Address of the module")]
        public string IPAddress { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the infrared module is currently connected.
        /// </summary>
        [Display(Name = "Is module connected")]
        public bool IsConnected { get; set; }
    }
}
