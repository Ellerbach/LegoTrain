using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoTrain.Models
{
    public sealed class ParamTrain
    {
        private const byte _NUMBER_TRAIN_MAX = 8;

        public byte Channel { get; set; }

        public ParamTrainRedBlue RedBlue { get; set; }

        public string TrainName { get; set; }

        public byte Speed { get; set; }

        public static byte NUMBER_TRAIN_MAX
        {
            get
            {
                return _NUMBER_TRAIN_MAX;
            }
        }
    }
}
