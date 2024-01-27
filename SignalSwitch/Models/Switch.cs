// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using Iot.Device.ServoMotor;
using System;
using System.Device.Pwm;

namespace LegoElement.Models
{
    public class Switch : IDisposable
    {
        private readonly ServoMotor _servoMotor;
        private bool _isStraight = false;
        private int _minPulse;
        private int _maxPulse;

        public Switch(int pinServo, int minPulse, int maxPulse)
        {
            PwmChannel pwm = PwmChannel.CreateFromPin(pinServo, frequency: 50);
            _servoMotor = new ServoMotor(pwm, 180, minPulse, maxPulse);
            _minPulse = minPulse;
            _maxPulse = maxPulse;
            _servoMotor.Start();
            SetStraight();
        }

        public void SetStraight()
        {
            _isStraight = true;
            _servoMotor.WritePulseWidth(_minPulse);
        }

        public void SetTurn()
        {
            _isStraight = false;
            _servoMotor.WritePulseWidth(_maxPulse);
        }

        public bool IsStraight { get => _isStraight; }

        public void Dispose()
        {
            _servoMotor?.Stop();
            _servoMotor?.Dispose();
        }
    }
}
