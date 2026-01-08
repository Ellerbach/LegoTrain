// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using LegoTrain.Models;

namespace LegoTrain.Services
{
    /// <summary>
    /// Interface for managing signal devices on the Lego train circuit.
    /// </summary>
    public interface ISignalManagement
    {
        /// <summary>
        /// Changes the state of a signal.
        /// </summary>
        /// <param name="NumSignal">The signal number.</param>
        /// <param name="value">The new signal state.</param>
        void ChangeSignal(byte NumSignal, SignalState value);
        /// <summary>
        /// Releases resources used by the signal management service.
        /// </summary>
        void Dispose();
        /// <summary>
        /// Gets the current state of a signal.
        /// </summary>
        /// <param name="NumSignal">The signal number.</param>
        /// <returns>The current signal state.</returns>
        SignalState GetSignal(byte NumSignal);
    }
}