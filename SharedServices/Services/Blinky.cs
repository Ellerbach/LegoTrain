// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using System;
using System.Device.Gpio;
using System.Threading;

namespace SharedServices.Services
{
    /// <summary>
    /// Provides LED blinking functionality for visual status indication on nanoFramework devices.
    /// </summary>
    public class Blinky : IDisposable
    {
        private readonly GpioController _gpio;
        private GpioPin _ledPin;
        private CancellationTokenSource _csToken;
        private Thread _thread;

        /// <summary>
        /// Initializes a new instance of the <see cref="Blinky"/> class.
        /// </summary>
        /// <param name="ledPin">The GPIO pin number for the LED.</param>
        public Blinky(int ledPin)
        {
            if (ledPin >= 0)
            {
                _gpio = new GpioController();
                _ledPin = _gpio.OpenPin(ledPin, PinMode.Output);
            }
        }

        /// <summary>
        /// Starts blinking the LED at normal speed (1 second interval).
        /// </summary>
        public void BlinkNormal() => Blink(1000);

        /// <summary>
        /// Starts blinking the LED at fast speed (100ms interval) to indicate waiting for WiFi connection.
        /// </summary>
        public void BlinkWaiWifi() => Blink(100);

        /// <summary>
        /// Releases all resources used by the Blinky instance.
        /// </summary>
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
