// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using System;

namespace nanoDiscovery.Common
{
    /// <summary>
    /// Represents the capabilities that a device can have.
    /// This is a flags enumeration allowing multiple capabilities to be combined.
    /// </summary>
    [Flags]
    public enum DeviceCapability
    {
        /// <summary>
        /// Device has no capabilities.
        /// </summary>
        None = 0b0000_0000,
        /// <summary>
        /// Device can control a signal.
        /// </summary>
        Signal = 0b0000_0001,
        /// <summary>
        /// Device can control a track switch.
        /// </summary>
        Switch = 0b0000_0010,
        /// <summary>
        /// Device can send infrared commands.
        /// </summary>
        Infrared = 0b0000_0100,
        /// <summary>
        /// Device has a train detector.
        /// </summary>
        Detector = 0b0000_1000,
        /// <summary>
        /// Device has a double train detector.
        /// </summary>
        DoubleDetector = 0b0001_0000,
        /// <summary>
        /// Device can control a double signal.
        /// </summary>
        DoubleSignal = 0b0010_0000,
    }
}
