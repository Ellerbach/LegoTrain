using System;

namespace LegoTrain
{
    public sealed class ParamRail
    {
        private byte numberOfTrains;
        private byte numberOfSignals;
        private byte numberOfSwitchs;
        private ParamTrain[] trains;
        private ParamSwitchs[] switchs;
        private string securityKey;
        private bool serial;
        private bool webServer;
        private uint minDuration;
        private uint maxDuration;
        private uint minAngle;
        private uint maxAngle;
        private uint servoAngle;

        public byte NumberOfTrains
        {
            get
            {
                return numberOfTrains;
            }

            set
            {
                numberOfTrains = value;
            }
        }

        public byte NumberOfSignals
        {
            get
            {
                return numberOfSignals;
            }

            set
            {
                numberOfSignals = value;
            }
        }

        public byte NumberOfSwitchs
        {
            get
            {
                return numberOfSwitchs;
            }

            set
            {
                numberOfSwitchs = value;
            }
        }

        public ParamTrain[] Trains
        {
            get
            {
                return trains;
            }

            set
            {
                trains = value;
            }
        }

        public ParamSwitchs[] Switchs
        {
            get
            {
                return switchs;
            }

            set
            {
                switchs = value;
            }
        }

        public string SecurityKey
        {
            get
            {
                return securityKey;
            }

            set
            {
                securityKey = value;
            }
        }

        public bool Serial
        {
            get
            {
                return serial;
            }

            set
            {
                serial = value;
            }
        }

        public bool WebServer
        {
            get
            {
                return webServer;
            }

            set
            {
                webServer = value;
            }
        }

        public uint MinDuration
        {
            get
            {
                return minDuration;
            }

            set
            {
                minDuration = value;
            }
        }

        public uint MaxDuration
        {
            get
            {
                return maxDuration;
            }

            set
            {
                maxDuration = value;
            }
        }

        public uint MinAngle
        {
            get
            {
                return minAngle;
            }

            set
            {
                minAngle = value;
            }
        }

        public uint MaxAngle
        {
            get
            {
                return maxAngle;
            }

            set
            {
                maxAngle = value;
            }
        }

        public uint ServoAngle
        {
            get
            {
                return servoAngle;
            }

            set
            {
                servoAngle = value;
            }
        }
    }

    public sealed class ParamTrain
    {
        private byte channel;
        private ParamTrainRedBlue redBlue;
        private string trainName;
        private byte speed;
        private const byte nUMBER_TRAIN_MAX = 8;

        public byte Channel
        {
            get
            {
                return channel;
            }

            set
            {
                channel = value;
            }
        }

        public ParamTrainRedBlue RedBlue
        {
            get
            {
                return redBlue;
            }

            set
            {
                redBlue = value;
            }
        }

        public string TrainName
        {
            get
            {
                return trainName;
            }

            set
            {
                trainName = value;
            }
        }

        public byte Speed
        {
            get
            {
                return speed;
            }

            set
            {
                speed = value;
            }
        }

        public static byte NUMBER_TRAIN_MAX
        {
            get
            {
                return nUMBER_TRAIN_MAX;
            }
        }
    }

    public sealed class ParamSwitchs
    {
        private string name;
        private int left;
        private int top;

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public int Left
        {
            get
            {
                return left;
            }

            set
            {
                left = value;
            }
        }

        public int Top
        {
            get
            {
                return top;
            }

            set
            {
                top = value;
            }
        }
    }

    public enum ParamTrainRedBlue { Red = 0, Blue = 1 };

}
