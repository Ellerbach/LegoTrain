// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using System;
using System.Device.Gpio;

namespace LegoElement.Models
{
    /// <summary>
    /// Represents a signal device that controls red and green LEDs.
    /// </summary>
    public class Signal : IDisposable
    {
        private readonly GpioController _gpio;
        private readonly int _pinRed;
        private readonly int _pinGreen;
        /// <summary>
        /// Gets the current state of the signal.
        /// </summary>
        public SignalState State { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Signal"/> class.
        /// </summary>
        /// <param name="pinRed">The GPIO pin for the red LED.</param>
        /// <param name="pinGreen">The GPIO pin for the green LED.</param>
        /// <exception cref="ArgumentException">Thrown when both pin parameters are negative.</exception>
        public Signal(int pinRed, int pinGreen)
        {
            if((pinRed < 0) && (pinGreen < 0))
            {
                throw new ArgumentException();
            }

            _pinRed = pinRed;
            _pinGreen = pinGreen;            
            _gpio = new GpioController();
            _gpio.OpenPin(_pinRed, PinMode.Output);
            _gpio.OpenPin(_pinGreen, PinMode.Output);
            SetBlack();
        }

        /// <summary>
        /// Sets the signal to green (go).
        /// </summary>
        public void SetGreen()
        {
            State = SignalState.Green;
            _gpio.Write(_pinGreen, PinValue.High);
            _gpio.Write(_pinRed, PinValue.Low);
        }

        /// <summary>
        /// Sets the signal to red (stop).
        /// </summary>
        public void SetRed()
        {
            State = SignalState.Red;
            _gpio.Write(_pinGreen, PinValue.Low);
            _gpio.Write(_pinRed, PinValue.High);
        }

        /// <summary>
        /// Sets the signal to black (off).
        /// </summary>
        public void SetBlack()
        {
            State = SignalState.Black;
            _gpio.Write(_pinGreen, PinValue.Low);
            _gpio.Write(_pinRed, PinValue.Low);
        }

        /// <summary>
        /// Releases all resources used by the signal.
        /// </summary>
        public void Dispose()
        {
            _gpio?.Dispose();
        }
    }
}
