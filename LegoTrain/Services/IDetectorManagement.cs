// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using LegoTrain.Models;
using LegoTrain.Models.Device;

namespace LegoTrain.Services
{
    /// <summary>
    /// Interface for managing detector devices on the Lego train circuit.
    /// </summary>
    public interface IDetectorManagement
    {
        /// <summary>
        /// Connects to a detector device and starts monitoring it.
        /// </summary>
        /// <param name="device">The device details.</param>
        /// <param name="token">The cancellation token source.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ConnectAndStart(DeviceDetails device, CancellationTokenSource token);
        /// <summary>
        /// Stops monitoring a detector device.
        /// </summary>
        /// <param name="device">The device to stop.</param>
        void Stop(DeviceDetails device);

        /// <summary>
        /// Delegate for detector events.
        /// </summary>
        /// <param name="detector">The detector that triggered the event.</param>
        delegate void DetectorEvent(Detector detector);
        /// <summary>
        /// Event raised when a detector event occurs.
        /// </summary>
        event DetectorEvent OnDetectorEvent;
    }
}
