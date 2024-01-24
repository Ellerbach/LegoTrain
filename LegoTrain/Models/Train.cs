// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using Lego.Infrared;
using System.ComponentModel.DataAnnotations;

namespace LegoTrain.Models
{
    public class Train
    {
        public const int MaximumNumberOfTrains = 8;

        [Display(Name = "Train's ID")]
        public int Id { get; set; }

        [Display(Name = "Infrared channel")]
        public Channel Channel { get; set; }

        [Display(Name = "Red or blue")]
        public PwmOutput Output { get; set; }

        [Display(Name = "Train's name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Nominal speed")]
        public PwmSpeed NominalSpeed { get; set; }
    }
}
