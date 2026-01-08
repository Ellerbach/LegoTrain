// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.


namespace LegoElement.Models
{
    /// <summary>
    /// Represents the possible states of a signal device.
    /// </summary>
    public enum SignalState
    {
        /// <summary>
        /// Signal is off (black).
        /// </summary>
        Black = 0,
        /// <summary>
        /// Signal shows red (stop).
        /// </summary>
        Red = 1,
        /// <summary>
        /// Signal shows green (go).
        /// </summary>
        Green = 2,
    }
}
