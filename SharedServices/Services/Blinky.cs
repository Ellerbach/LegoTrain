// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using System;
using System.Device.Gpio;
using System.Threading;

namespace SharedServices.Services
{
    public class Blinky : IDisposable
    {
        private readonly GpioController _gpio;
        private GpioPin _ledPin;
        private CancellationTokenSource _csToken;
        private Thread _thread;

        public Blinky(int ledPin)
        {
            if (ledPin >= 0)
            {
                _gpio = new GpioController();
                _ledPin = _gpio.OpenPin(ledPin, PinMode.Output);
            }
        }

        public void BlinkNormal() => Blink(1000);

        public void BlinkWaiWifi() => Blink(100);

        public void Dispose()
        {
            ResetToken();
            _ledPin?.Dispose();
            _ledPin = null;
            _gpio?.Dispose();
        }

        private void Blink(int millisec)
        {
            if (_ledPin == null)
            {
                return;
            }

            ResetToken();
            _thread = new Thread(() =>
            {
                while (!_csToken.IsCancellationRequested)
                {
                    if (_ledPin == null)
                    {
                        return;
                    }

                    _ledPin.Toggle();
                    _csToken.Token.WaitHandle.WaitOne(millisec, true);
                }
            });
            _thread.Start();
        }

        private void ResetToken()
        {
            if (_csToken != null && !_csToken.IsCancellationRequested)
            {
                _csToken.Cancel();
            }

            if (_thread != null)
            {
                _thread.Join(1000);
            }

            _csToken = new CancellationTokenSource();
        }
    }
}
