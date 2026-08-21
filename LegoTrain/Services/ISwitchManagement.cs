// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

namespace LegoTrain.Services
{
    /// <summary>
    /// Interface for managing track switch devices on the Lego train circuit.
    /// </summary>
    public interface ISwitchManagement
    {
        /// <summary>
        /// Changes the state of a switch.
        /// </summary>
        /// <param name="NumSignal">The switch number.</param>
        /// <param name="value">The new switch state (true for one position, false for the other).</param>
        void ChangeSwitch(byte NumSignal, bool value);
        /// <summary>
        /// Releases resources used by the switch management service.
        /// </summary>
        void Dispose();
        /// <summary>
        /// Gets the current state of a switch.
        /// </summary>
        /// <param name="NumSignal">The switch number.</param>
        /// <returns>The current switch state.</returns>
        bool GetSwitch(byte NumSignal);
    }
}