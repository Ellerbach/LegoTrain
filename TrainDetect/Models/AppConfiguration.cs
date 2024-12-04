// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using nanoFramework.Json;
using SharedServices.Models;
using System.IO;

namespace LegoElement.Models
{
    public class AppConfiguration : IAppConfiguration
    {
        private const string FileName = "I:\\config.json";
        private int _deviceId = -1;
        private bool _firstDetector = false;
        private bool _secondDetector = false;
        private int _detector1MinThreshold = -1;
        private int _detector1MaxThreshold = -1;
        private int _detector2MinThreshold = -1;
        private int _detector2MaxThreshold = -1;
        private int _detector1Pin1 = -1;
        private int _detector2Pin1 = -1;
        private int _gpioLed = -1;

        public delegate void ConfigurationUpdated(object sender, ConfigurationEventArgs e);
        public event ConfigurationUpdated OnConfigurationUpdated;

        public static AppConfiguration Load()
        {
            if (!File.Exists(FileName))
            {
                return null;
            }

            string config = File.ReadAllText(FileName);
            var configuration = (AppConfiguration)JsonConvert.DeserializeObject(config, typeof(AppConfiguration));
            return configuration;
        }

        public void Save()
        {
            var config = JsonConvert.SerializeObject(this);
            File.WriteAllText(FileName, config);
        }

        public AppConfiguration()
        {
        }

        public int DeviceId
        {
            get => _deviceId;
            set
            {
                if (value == _deviceId)
                {
                    return;
                }

                _deviceId = value;
                OnConfigurationUpdated?.Invoke(this, new ConfigurationEventArgs(nameof(DeviceId)));
            }
        }

        public bool FirstDetectorActivated
        {
            get => _firstDetector;
            set
            {
                if (_firstDetector == value)
                {
                    return;
                }

                _firstDetector = value;
                OnConfigurationUpdated?.Invoke(this, new ConfigurationEventArgs(nameof(FirstDetectorActivated)));
            }
        }

        public int Detector1Pin
        {
            get => _detector1Pin1;
            set
            {
                if (value == _detector1Pin1)
                {
                    return;
                }

                _detector1Pin1 = value;
                OnConfigurationUpdated?.Invoke(this, new ConfigurationEventArgs(nameof(Detector1Pin)));
            }
        }

        public int Detector1MinimumThreshold
        {
            get => _detector1MinThreshold;
            set
            {
                if (value == _detector1MinThreshold)
                {
                    return;
                }

                _detector1MinThreshold = value;
                OnConfigurationUpdated?.Invoke(this, new ConfigurationEventArgs(nameof(Detector1MinimumThreshold)));
            }
        }

        public int Detector1MaximumThreshold
        {
            get => _detector1MaxThreshold;
            set
            {
                if (value == _detector1MaxThreshold)
                {
                    return;
                }

                _detector1MaxThreshold = value;
                OnConfigurationUpdated?.Invoke(this, new ConfigurationEventArgs(nameof(Detector1MaximumThreshold)));
            }
        }

        public bool SecondDetectorActivated
        {
            get => _secondDetector;
            set
            {
                if (_secondDetector == value)
                {
                    return;
                }

                _secondDetector = value;
                OnConfigurationUpdated?.Invoke(this, new ConfigurationEventArgs(nameof(SecondDetectorActivated)));
            }
        }

        public int Detector2Pin
        {
            get => _detector2Pin1;
            set
            {
                if (_detector2Pin1 == value)
                {
                    return;
                }

                _detector2Pin1 = value;
                OnConfigurationUpdated?.Invoke(this, new ConfigurationEventArgs(nameof(Detector2Pin)));
            }
        }

        public int Detector2MinimumThreshold
        {
            get => _detector2MinThreshold;
            set
            {
                if (value == _detector2MinThreshold)
                {
                    return;
                }

                _detector2MinThreshold = value;
                OnConfigurationUpdated?.Invoke(this, new ConfigurationEventArgs(nameof(Detector1MinimumThreshold)));
            }
        }

        public int Detector2MaximumThreshold
        {
            get => _detector2MaxThreshold;
            set
            {
                if (value == _detector2MaxThreshold)
                {
                    return;
                }

                _detector2MaxThreshold = value;
                OnConfigurationUpdated?.Invoke(this, new ConfigurationEventArgs(nameof(Detector1MaximumThreshold)));
            }
        }

        public int LedGpio
        {
            get => _gpioLed;
            set
            {
                if (_gpioLed == value)
                {
                    return;
                }

                _gpioLed = value;
                OnConfigurationUpdated?.Invoke(this, new ConfigurationEventArgs(nameof(LedGpio)));
            }
        }

        public int ApiPort { get; set; }

        public bool StartDetectionAutomatically { get; set; }
    }
}
