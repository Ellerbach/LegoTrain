// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using System;
using System.Device.Gpio;

namespace LegoElement.Models
{
    public class Signal : IDisposable
    {
        private readonly GpioController _gpio;
        private readonly int _pinRed;
        private readonly int _pinGreen;
        public SignalState State { get; internal set; }

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

        public void SetGreen()
        {
            State = SignalState.Green;
            _gpio.Write(_pinGreen, PinValue.High);
            _gpio.Write(_pinRed, PinValue.Low);
        }

        public void SetRed()
        {
            State = SignalState.Red;
            _gpio.Write(_pinGreen, PinValue.Low);
            _gpio.Write(_pinRed, PinValue.High);
        }

        public void SetBlack()
        {
            State = SignalState.Black;
            _gpio.Write(_pinGreen, PinValue.Low);
            _gpio.Write(_pinRed, PinValue.Low);
        }

        public void Dispose()
        {
            _gpio?.Dispose();
        }
    }
}
