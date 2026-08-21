// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using nanoFramework.Hardware.Esp32;
using nanoFramework.Runtime.Native;
using System;
using System.Device.Adc;
using System.Device.Gpio;
using System.Threading;

namespace LegoElement.Models
{
    /// <summary>
    /// Represents a train detector that monitors an ADC channel for train presence.
    /// </summary>
    public class Detector : IDisposable
    {
        private AdcController _controller = new AdcController();
        private AdcChannel _channel;
        private Thread _thread;
        private volatile bool _running;

        /// <summary>
        /// Delegate for detector events.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The ADC value.</param>
        /// <param name="detected">True if train is detected, false otherwise.</param>
        public delegate void DetectorDetected(object sender, int e, bool detected);
        /// <summary>
        /// Event raised when the detection state changes.
        /// </summary>
        public event DetectorDetected OnDetection;

        /// <summary>
        /// Initializes a new instance of the <see cref="Detector"/> class.
        /// </summary>
        /// <param name="pinAdc">The ADC pin number.</param>
        /// <param name="id">The detector identifier (0 or 1).</param>
        /// <param name="threshold">The detection threshold value. Default is -1.</param>
        /// <exception cref="Exception">Thrown when pin or id parameters are invalid.</exception>
        public Detector(int pinAdc, int id, int threshold = -1)
        {
            if ((pinAdc < 0) || (id < 0) || (id > 1))
            {
                throw new Exception();
            }

            Threshold = threshold;
            Id = id;
            if (SystemInfo.TargetName == "XIAO_ESP32C3")
            {
                // On ESP32S3, the ADC channel is the pin number from 0 to 20
                // Less than 10 needs to use if wifi is used
                _channel = _controller.OpenChannel(pinAdc);
            }
            else
            {
                // TODO but it will most likely fail
                Configuration.SetPinFunction(pinAdc, id == 0 ? DeviceFunction.ADC1_CH0 : DeviceFunction.ADC1_CH1);
                _channel = _controller.OpenChannel(id);
            }

            Detect = false;
            IsDetected = false;
            Start();
        }

        /// <summary>
        /// Gets a value indicating whether a train is currently detected.
        /// </summary>
        public bool IsDetected { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether detection is currently enabled.
        /// </summary>
        public bool Detect { get; set; }

        /// <summary>
        /// Starts the detector thread.
        /// </summary>
        public void Start()
        {
            if (_running)
            {
                return;
            }

            _running = true;
            _thread = new Thread(() =>
            {
                while (_running)
                {
                    Value = _channel.ReadValue();
                    if (Detect)
                    {
                        if ((Value <= Threshold) && !IsDetected)
                        {
                            IsDetected = true;
                            OnDetection?.Invoke(this, Value, IsDetected);
                        }

                        if ((Value > Threshold) && IsDetected)
                        {
                            IsDetected = false;
                            OnDetection?.Invoke(this, Value, IsDetected);
                        }
                    }

                    Thread.Sleep(20);
                }
            });
            _thread.Start();
        }

        /// <summary>
        /// Stops the detector thread.
        /// </summary>
        public void Stop()
        {
            if (!_running)
            {
                return;
            }

            _running = false;
            if (_thread != null)
            {
                _thread.Join();
                _thread = null;
            }
        }

        /// <summary>
        /// Gets the detector identifier.
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// Gets or sets the detection threshold value.
        /// </summary>
        public int Threshold { get; set; }

        /// <summary>
        /// Gets the current ADC reading value.
        /// </summary>
        public int Value { get; internal set; }

        /// <summary>
        /// Releases all resources used by the detector.
        /// </summary>
        public void Dispose()
        {
            Stop();
            if (_channel != null)
            {
                _channel.Dispose();
            }
        }
    }
}
