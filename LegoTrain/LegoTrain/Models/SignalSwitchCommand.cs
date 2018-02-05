using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoTrain.Models
{
    public class SignalSwitchCommand
    {
        public byte SignaSwitchNumber { get; set; }
        //Green = true; Red = false
        //Straight = true; turn = false
        public bool State { get; set; }
    }
}
