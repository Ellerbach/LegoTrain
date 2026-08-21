// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using Lego.Infrared;
using System.ComponentModel.DataAnnotations;

namespace LegoTrain.Models
{
    /// <summary>
    /// Represents a train on the Lego train circuit.
    /// </summary>
    public class Train
    {
        /// <summary>
        /// The maximum number of trains that can be configured.
        /// </summary>
        public const int MaximumNumberOfTrains = 8;

        /// <summary>
        /// Gets or sets the unique identifier for the train.
        /// </summary>
        [Display(Name = "Train's ID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the infrared channel used to control this train.
        /// </summary>
        [Display(Name = "Infrared channel")]
        public Channel Channel { get; set; }

        /// <summary>
        /// Gets or sets the PWM output (Red or Blue) for the train.
        /// </summary>
        [Display(Name = "Red or blue")]
        public PwmOutput Output { get; set; }

        /// <summary>
        /// Gets or sets the name of the train.
        /// </summary>
        [Display(Name = "Train's name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the nominal speed for the train.
        /// </summary>
        [Display(Name = "Nominal speed")]
        public PwmSpeed NominalSpeed { get; set; }
    }
}
