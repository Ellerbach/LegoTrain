// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

namespace LegoTrain.Models.Device
{
    /// <summary>
    /// Represents the connection status of a device.
    /// </summary>
    public enum DeviceStatus
    {
        /// <summary>
        /// Device is joining the network.
        /// </summary>
        Joining,
        /// <summary>
        /// Device is leaving the network.
        /// </summary>
        Laaving,
        /// <summary>
        /// Device is absent from the network.
        /// </summary>
        Absent,
    }
}
