﻿// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using nanoFramework.Json;
using SharedServices.Models;
using System.IO;

namespace LegoElement.Models
{
    public class AppConfiguration : IAppConfiguration
    {
        public const string Signal = "Signal";
        public const string Switch = "Switch";
        public const string Both = "Both";

        private const string FileName = "I:\\config.json";
        private int _deviceId = -1;
        private string _deviceType = string.Empty;
        private int _minPulse = -1;
        private int _maxPulse = -1;
        private int _servoPin = -1;
        private int _gpioRed = -1;
        private int _gpioGreen = -1;

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

        public string DeviceType
        {
            get => _deviceType;
            set
            {
                if (_deviceType == value)
                {
                    return;
                }

                string deviceType = value.ToLower();
                if (deviceType == Signal.ToLower())
                {
                    _deviceType = Signal;
                }
                else if (deviceType == Switch.ToLower())
                {
                    _deviceType = Switch;
                }
                else if (deviceType == Both.ToLower())
                {
                    _deviceType = Both;
                }
                else
                {    // Not valid
                    return;
                }

                OnConfigurationUpdated?.Invoke(this, new ConfigurationEventArgs(nameof(DeviceType)));
            }
        }

        public int ServoMinimumPulse
        {
            get => _minPulse;
            set
            {
                if (value == _minPulse)
                {
                    return;
                }

                _minPulse = value;
                OnConfigurationUpdated?.Invoke(this, new ConfigurationEventArgs(nameof(ServoMinimumPulse)));
            }
        }

        public int ServoMaximumPulse
        {
            get => _maxPulse;
            set
            {
                if (value == _maxPulse)
                {
                    return;
                }

                _maxPulse = value;
                OnConfigurationUpdated?.Invoke(this, new ConfigurationEventArgs(nameof(ServoMaximumPulse)));
            }
        }

        public int ServoGpio
        {
            get => _servoPin;
            set
            {
                if (value == _servoPin)
                {
                    return;
                }

                _servoPin = value;
                OnConfigurationUpdated?.Invoke(this, new ConfigurationEventArgs(nameof(ServoGpio)));
            }
        }

        public int SignalGpioRed
        {
            get => _gpioRed;
            set
            {
                if (_gpioRed == value)
                {
                    return;
                }

                _gpioRed = value;
                OnConfigurationUpdated?.Invoke(this, new ConfigurationEventArgs(nameof(SignalGpioRed)));
            }
        }

        public int SignalGpioGreen
        {
            get => _gpioGreen;
            set
            {
                if (_gpioGreen == value)
                {
                    return;
                }

                _gpioGreen = value;
                OnConfigurationUpdated?.Invoke(this, new ConfigurationEventArgs(nameof(SignalGpioGreen)));
            }
        }
    }
}
