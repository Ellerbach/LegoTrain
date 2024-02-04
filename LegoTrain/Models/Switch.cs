// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;

namespace LegoTrain.Models
{
    public class Switch
    {
        [Display(Name = "Switch's ID")]
        public int Id { get; set; }

        [Display(Name = "Switch's name")]
        public string Name { get; set; } = string.Empty;


        [Display(Name = "X position on the circuit")]
        public int X { get; set; }

        [Display(Name = "Y position on the circuit")]
        public int Y { get; set; }

        [Display(Name = "Check for left switch")]
        public bool IsLeft { get; set; }

        [Display(Name = "IP Address of the module")]
        public string IPAddress { get; set; } = string.Empty;

        [Display(Name = "Is module connected")]
        public bool IsConnected { get; set; }

        [Display(Name = "Rotation in degrees")]
        public int Rotation { get; set; }
    }
}
