// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;

namespace LegoTrain.Models
{
    public class Infrared
    {
        [Display(Name = "IP Address of the module")]
        public string IPAddress { get; set; } = string.Empty;

        [Display(Name = "Is module connected")]
        public bool IsConnected { get; set; }
    }
}
