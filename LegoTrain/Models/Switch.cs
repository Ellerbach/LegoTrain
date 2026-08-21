// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;

namespace LegoTrain.Models
{
    /// <summary>
    /// Represents a track switch device on the Lego train circuit.
    /// </summary>
    public class Switch
    {
        /// <summary>
        /// Gets or sets the unique identifier for the switch.
        /// </summary>
        [Display(Name = "Switch's ID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the switch.
        /// </summary>
        [Display(Name = "Switch's name")]
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
        /// Gets or sets a value indicating whether this is a left-oriented switch.
        /// </summary>
        [Display(Name = "Check for left switch")]
        public bool IsLeft { get; set; }

        /// <summary>
        /// Gets or sets the IP address of the switch module.
        /// </summary>
        [Display(Name = "IP Address of the module")]
        public string IPAddress { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the switch module is currently connected.
        /// </summary>
        [Display(Name = "Is module connected")]
        public bool IsConnected { get; set; }

        /// <summary>
        /// Gets or sets the rotation angle of the switch in degrees.
        /// </summary>
        [Display(Name = "Rotation in degrees")]
        public int Rotation { get; set; }
    }
}
