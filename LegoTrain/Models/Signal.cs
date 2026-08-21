// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;

namespace LegoTrain.Models
{
    /// <summary>
    /// Represents a signal device on the Lego train circuit.
    /// </summary>
    public class Signal
    {
        /// <summary>
        /// Gets or sets the unique identifier for the signal.
        /// </summary>
        [Display(Name = "Signal's ID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the device identifier associated with this signal.
        /// </summary>
        [Display(Name = "Device's ID")]
        public int DeviceId { get; set; }

        /// <summary>
        /// Gets or sets the name of the signal.
        /// </summary>
        [Display(Name = "Signal's name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the X coordinate position on the circuit map.
        /// </summary>
        [Display(Name = "X position on the circuit")]
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the Y coordinate position on the circuit map.
        /// </summary>
        [Display(Name = "Y position on the circuit")]
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the IP address of the signal module.
        /// </summary>
        [Display(Name = "IP Address of the module")]
        public string IPAddress { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the signal module is currently connected.
        /// </summary>
        [Display(Name = "Is module connected")]
        public bool IsConnected { get; set; }
    }
}
