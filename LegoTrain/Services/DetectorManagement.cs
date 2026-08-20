// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using LegoTrain.Models;
using LegoTrain.Models.Device;
using nanoDiscovery.Common;
using System.Net.WebSockets;
using System.Text;

namespace LegoTrain.Services
{
    /// <summary>
    /// Manages detector devices on the Lego train circuit, handling connections and events.
    /// </summary>
    public class DetectorManagement : IDetectorManagement, IDisposable
    {
        private static readonly HttpClient _client = new HttpClient();
        private readonly AppConfiguration _config;
        private readonly object _devicesLock = new object();
        private LegoDiscovery _disco;
        private Dictionary<Detector, CancellationTokenSource> _devices = new Dictionary<Detector, CancellationTokenSource>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DetectorManagement"/> class.
        /// </summary>
        /// <param name="legoDiscovery">The device discovery service.</param>
        /// <param name="config">The application configuration containing the detector list.</param>
        public DetectorManagement(LegoDiscovery legoDiscovery, AppConfiguration config)
        {
            _disco = legoDiscovery;
            _config = config;
            _disco.OnDeviceJoined += OnDeviceJoined;
            _disco.OnDeviceLeft += OnDeviceLeft;
        }

        private void OnDeviceLeft(DeviceDetails device)
        {
            if (device.DeviceCapacity.HasFlag(DeviceCapability.Detector) || device.DeviceCapacity.HasFlag(DeviceCapability.DoubleDetector))
            {
                List<CancellationTokenSource> toCancel = new List<CancellationTokenSource>();

                lock (_devicesLock)
                {
                    foreach (var pair in _devices)
                    {
                        if (pair.Key.Id == device.Id)
                        {
                            toCancel.Add(pair.Value);
                            _devices.Remove(pair.Key);
                        }
                    }
                }

                foreach (var token in toCancel)
                {
                    token.Cancel();
                }
            }
        }

        private void OnDeviceJoined(DeviceDetails device)
        {
            if (device.DeviceCapacity.HasFlag(DeviceCapability.Detector) || device.DeviceCapacity.HasFlag(DeviceCapability.DoubleDetector))
            {
                var detector = new Detector();
                detector.IPAddress = device.IPAddress.ToString();
                detector.IsConnected = device.DeviceStatus == DeviceStatus.Joining;
                detector.Id = device.Id;
                var token = new CancellationTokenSource();

                lock (_devicesLock)
                {
                    _devices[detector] = token;
                }

                Task.Run(async () => await ConnectAndStart(device, token));
            }
        }

        /// <summary>
        /// Event raised when a detector triggers.
        /// </summary>
        public event IDetectorManagement.DetectorEvent OnDetectorEvent;

        /// <summary>
        /// Updates the detector model and raises the event for the reported detector state.
        /// </summary>
        /// <param name="deviceId">The owning device identifier.</param>
        /// <param name="detectorId">The local detector channel identifier on the device.</param>
        /// <param name="value">The reported detector value.</param>
        /// <param name="triggered">Whether the detector is active.</param>
        public void UpdateDetectorState(int deviceId, int detectorId, int value, bool triggered)
        {
            var detector = _config.Detectors.FirstOrDefault(d => d.DeviceId == deviceId && d.Id == detectorId);
            if (detector == null)
            {
                detector = new Detector
                {
                    DeviceId = deviceId,
                    Id = detectorId,
                    Name = $"Detector {deviceId}:{detectorId}",
                    IsConnected = true
                };
                _config.Detectors.Add(detector);
            }

            detector.DeviceId = deviceId;
            detector.Value = value;
            detector.Triggered = triggered;
            detector.IsConnected = true;
            OnDetectorEvent?.Invoke(detector);
        }

        /// <summary>
        /// Connects to a detector device via WebSocket and starts receiving detection events.
        /// </summary>
        /// <param name="device">The device to connect to.</param>
        /// <param name="token">The cancellation token source for stopping the connection.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ConnectAndStart(DeviceDetails device, CancellationTokenSource token)
        {
            using (ClientWebSocket client = new ClientWebSocket())
            {
                Uri serverUri = new Uri($"ws://{device.IPAddress}");
                await client.ConnectAsync(serverUri, CancellationToken.None);
                Console.WriteLine("Connected to the server!");

                // Sending a message to the server
                string message = "start";
                ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
                await client.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
                Console.WriteLine("Message sent to the server!");

                byte[] buffer = new byte[1024];
                while (!token.IsCancellationRequested)
                {
                    // Receiving a message from the server
                    WebSocketReceiveResult result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), token.Token);
                    string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine("Message received from the server: " + receivedMessage);

                    if (string.IsNullOrWhiteSpace(receivedMessage))
                    {
                        continue;
                    }

                    var keyValues = receivedMessage.Split('&');
                    var payload = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var item in keyValues)
                    {
                        var parts = item.Split('=', 2);
                        if (parts.Length == 2)
                        {
                            payload[parts[0]] = parts[1];
                        }
                    }

                    if (payload.TryGetValue("id", out var deviceIdText)
                        && payload.TryGetValue("de", out var detectorIdText)
                        && payload.TryGetValue("va", out var valueText)
                        && int.TryParse(deviceIdText, out var deviceId)
                        && int.TryParse(detectorIdText, out var detectorId)
                        && int.TryParse(valueText, out var value))
                    {
                        var triggered = payload.TryGetValue("on", out var stateText)
                            && int.TryParse(stateText, out var state)
                            && state != 0;
                        UpdateDetectorState(deviceId, detectorId, value, triggered);
                    }
                }

                // Closing the connection
                await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", new CancellationTokenSource(5000).Token);
                Console.WriteLine("Connection closed.");
            }
        }

        /// <summary>
        /// Releases all resources used by the detector management service.
        /// </summary>
        public void Dispose()
        {
            List<CancellationTokenSource> toCancel = new List<CancellationTokenSource>();

            lock (_devicesLock)
            {
                foreach (var pair in _devices)
                {
                    toCancel.Add(pair.Value);
                }

                _devices.Clear();
            }

            foreach (var token in toCancel)
            {
                token.Cancel();
            }
        }

        /// <summary>
        /// Stops monitoring a specific detector device.
        /// </summary>
        /// <param name="device">The device to stop monitoring.</param>
        public void Stop(DeviceDetails device)
        {
            List<CancellationTokenSource> toCancel = new List<CancellationTokenSource>();

            lock (_devicesLock)
            {
                foreach (var pair in _devices)
                {
                    if (pair.Key.Id == device.Id)
                    {
                        toCancel.Add(pair.Value);
                        _devices.Remove(pair.Key);
                    }
                }
            }

            foreach (var token in toCancel)
            {
                token.Cancel();
            }
        }
    }
}
