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
