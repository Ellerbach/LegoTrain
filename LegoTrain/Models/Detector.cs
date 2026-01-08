// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;

namespace LegoTrain.Models
{
    /// <summary>
    /// Represents a train detector device on the Lego train circuit.
    /// </summary>
    public class Detector
    {
        /// <summary>
        /// Gets or sets the unique identifier for the detector.
        /// </summary>
        [Display(Name = "Dector's ID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the detector.
        /// </summary>
        [Display(Name = "Detector's name")]
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
        /// Gets or sets the IP address of the detector module.
        /// </summary>
        [Display(Name = "IP Address of the module")]
        public string IPAddress { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the detector module is currently connected.
        /// </summary>
        [Display(Name = "Is module connected")]
        public bool IsConnected { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether detection is enabled on this detector.
        /// </summary>
        [Display(Name = "Is detection enabled")]
        public bool Detect { get; set; }

        /// <summary>
        /// Gets or sets the current value reading from the detector.
        /// </summary>
        [Display(Name = "Value of the detector")]
        public int Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the detector has been triggered.
        /// </summary>
        [Display(Name = "Is detection triggered")]
        public bool Triggered { get; set; }
    }
}
