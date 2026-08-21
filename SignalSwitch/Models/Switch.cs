// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using Iot.Device.ServoMotor;
using System;
using System.Device.Pwm;

namespace LegoElement.Models
{
    /// <summary>
    /// Represents a track switch controlled by a servo motor.
    /// </summary>
    public class Switch : IDisposable
    {
        private readonly ServoMotor _servoMotor;
        private bool _isStraight = false;
        private int _minPulse;
        private int _maxPulse;

        /// <summary>
        /// Initializes a new instance of the <see cref="Switch"/> class.
        /// </summary>
        /// <param name="pinServo">The PWM pin for the servo motor.</param>
        /// <param name="minPulse">The minimum pulse width in microseconds.</param>
        /// <param name="maxPulse">The maximum pulse width in microseconds.</param>
        public Switch(int pinServo, int minPulse, int maxPulse)
        {
            PwmChannel pwm = PwmChannel.CreateFromPin(pinServo, frequency: 50);
            _servoMotor = new ServoMotor(pwm, 180, minPulse, maxPulse);
            _minPulse = minPulse;
            _maxPulse = maxPulse;
            _servoMotor.Start();
            SetStraight();
        }

        /// <summary>
        /// Sets the switch to the straight position.
        /// </summary>
        public void SetStraight()
        {
            _isStraight = true;
            _servoMotor.WritePulseWidth(_minPulse);
        }

        /// <summary>
        /// Sets the switch to the turn position.
        /// </summary>
        public void SetTurn()
        {
            _isStraight = false;
            _servoMotor.WritePulseWidth(_maxPulse);
        }

        /// <summary>
        /// Gets a value indicating whether the switch is in the straight position.
        /// </summary>
        public bool IsStraight { get => _isStraight; }

        /// <summary>
        /// Releases all resources used by the switch.
        /// </summary>
        public void Dispose()
        {
            _servoMotor?.Stop();
            _servoMotor?.Dispose();
        }
    }
}
