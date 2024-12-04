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
    public class Detector : IDisposable
    {
        private AdcController _controller = new AdcController();
        private AdcChannel _channel;
        private Thread _thread;

        public delegate void DetectorDetected(object sender, int e);
        public event DetectorDetected OnDetection;

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

            _thread = new Thread(() =>
            {
                bool detected = false;
                while (_thread.IsAlive)
                {
                    Value = _channel.ReadValue();
                    if (Detect && ((Value <= Threshold) && (!detected)))
                    {
                        detected = true;
                        OnDetection?.Invoke(this, Value);
                    }

                    if (Value > Threshold)
                    {
                        detected = false;
                    }

                    Thread.Sleep(20);
                }
            });
            _thread.Start();
        }

        public bool Detect { get; set; }

        public void Start()
        {
            _thread.Start();
        }

        public void Stop()
        {
            _thread.Abort();
            _thread.Join(100);
        }

        public int Id { get; internal set; }

        public int Threshold { get; set; }

        public int Value { get; internal set; }

        public void Dispose()
        {
            _thread.Abort();
            _thread.Join(100);
            _thread = null;
            _channel.Dispose();
        }
    }
}
