using System;

namespace LegoTrain.Models
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

        public byte NumberOfTrains { get; set; }

        public byte NumberOfSignals { get; set; }

        public byte NumberOfSwitchs { get; set; }

        public ParamTrain[] Trains { get; set; }

        public ParamSwitchs[] Switchs { get; set; }

        public ParamSignal[] Signals { get; set; }

        public string SecurityKey { get; set; }

        public bool Serial { get; set; }

        public bool WebServer { get; set; }

        public uint MinDuration { get; set; }

        public uint MaxDuration { get; set; }

        public uint MinAngle { get; set; }

        public uint MaxAngle { get; set; }

        public uint ServoAngle { get; set; }
    }

 


}
