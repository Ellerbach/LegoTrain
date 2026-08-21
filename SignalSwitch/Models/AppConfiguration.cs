// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using nanoFramework.Json;
using SharedServices.Models;
using System.IO;

namespace LegoElement.Models
{
    /// <summary>
    /// Application configuration for the SignalSwitch device.
    /// </summary>
    public class AppConfiguration : IAppConfiguration
    {
        /// <summary>
        /// Constant representing signal mode.
        /// </summary>
        public const string Signal = "Signal";
        /// <summary>
        /// Constant representing switch mode.
        /// </summary>
        public const string Switch = "Switch";
        /// <summary>
        /// Constant representing both signal and switch mode.
        /// </summary>
        public const string Both = "Both";

        private const string FileName = "I:\\config.json";
        private int _deviceId = -1;
        private bool _signalActivated = false;
        private bool _switchActivated = false;
        private int _minPulse = -1;
        private int _maxPulse = -1;
        private int _servoPin = -1;
        private int _gpioRed = -1;
        private int _gpioGreen = -1;
        private int _gpioLed = -1;

        /// <summary>
        /// Delegate for configuration update events.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        public delegate void ConfigurationUpdated(object sender, ConfigurationEventArgs e);
        /// <summary>
        /// Event raised when the configuration is updated.
        /// </summary>
        public event ConfigurationUpdated OnConfigurationUpdated;

        /// <summary>
        /// Loads the application configuration from the file system.
        /// </summary>
        /// <returns>The loaded configuration, or null if the file doesn't exist.</returns>
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

        /// <summary>
        /// Saves the current configuration to the file system.
        /// </summary>
        public void Save()
        {
            var config = JsonConvert.SerializeObject(this);
            File.WriteAllText(FileName, config);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfiguration"/> class.
        /// </summary>
        public AppConfiguration()
        {
        }

        /// <summary>
        /// Gets or sets the unique device identifier.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether the switch functionality is activated.
        /// </summary>
        public bool SwitchActivated
        {
            get => _switchActivated;
            set
            {
                if (_switchActivated == value)
                {
                    return;
                }

                _switchActivated = value;
                OnConfigurationUpdated?.Invoke(this, new ConfigurationEventArgs(nameof(SwitchActivated)));
            }
        }

        /// <summary>
        /// Gets or sets the minimum pulse width for the servo motor in microseconds.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the maximum pulse width for the servo motor in microseconds.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the GPIO pin number for the servo motor control.
        /// </summary>
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


        /// <summary>
        /// Gets or sets a value indicating whether the signal functionality is activated.
        /// </summary>
        public bool SignalActivated
        {
            get => _signalActivated;
            set
            {
                if (_signalActivated == value)
                {
                    return;
                }

                _signalActivated = value;
                OnConfigurationUpdated?.Invoke(this, new ConfigurationEventArgs(nameof(SignalActivated)));
            }
        }

        /// <summary>
        /// Gets or sets the GPIO pin number for the red signal LED.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the GPIO pin number for the green signal LED.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the GPIO pin number for the status LED indicator.
        /// </summary>
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
    }
}
