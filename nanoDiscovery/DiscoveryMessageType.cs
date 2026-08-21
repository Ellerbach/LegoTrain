// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

namespace nanoDiscovery
{
    /// <summary>
    /// Represents the type of discovery protocol message.
    /// </summary>
    public enum DiscoveryMessageType: byte
    {
        /// <summary>
        /// No message type.
        /// </summary>
        None = 0,
        /// <summary>
        /// Discovery broadcast message to find devices.
        /// </summary>
        Discovery = 1,
        /// <summary>
        /// Capabilities response message from a device.
        /// </summary>
        Capabilities = 2,
        /// <summary>
        /// Goodbye message when a device is leaving.
        /// </summary>
        Byebye = 3,
        /// <summary>
        /// Generic application message.
        /// </summary>
        Message = 4,
    }
}
