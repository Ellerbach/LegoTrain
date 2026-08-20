// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using LegoTrain.Models.Device;
using nanoDiscovery;
using nanoDiscovery.Common;
using System.Net;
using System.Net.Sockets;

namespace LegoTrain.Services
{
    /// <summary>
    /// Manages device discovery on the Lego train network using UDP broadcast messages.
    /// </summary>
    public class LegoDiscovery : IDisposable
    {
        private const int BindingPort = 2024;
        private UdpClient _udpClient;
        private Thread _runDiscovery;
        private CancellationTokenSource _runDiscoToken;
        private Thread _runReceive;
        private CancellationTokenSource _runReceiveToken;
        private List<DeviceDetails> _deviceDetails = new List<DeviceDetails>();
        private readonly object _deviceDetailsLock = new object();

        /// <summary>
        /// Gets the list of discovered devices.
        /// </summary>
        public List<DeviceDetails> DeviceDetails
        {
            get
            {
                lock (_deviceDetailsLock)
                {
                    return new List<DeviceDetails>(_deviceDetails);
                }
            }
        }

        /// <summary>
        /// Delegate for device events.
        /// </summary>
        /// <param name="device">The device that triggered the event.</param>
        public delegate void DeviceEvent(DeviceDetails device);
        /// <summary>
        /// Event raised when a device joins the network.
        /// </summary>
        public event DeviceEvent OnDeviceJoined;

        /// <summary>
        /// Event raised when a device leaves the network.
        /// </summary>
        public event DeviceEvent OnDeviceLeft;

        /// <summary>
        /// Initializes a new instance of the <see cref="LegoDiscovery"/> class.
        /// </summary>
        /// <param name="update">The time interval for sending discovery messages. Defaults to 1 minute.</param>
        public LegoDiscovery(TimeSpan update = default)
        {
            _udpClient = new UdpClient();
            _udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, BindingPort));
            _runDiscoToken = new CancellationTokenSource();
            _runReceiveToken = new CancellationTokenSource();
            if (update == default)
            {
                update = TimeSpan.FromMinutes(1);
            }

            _runDiscovery = new Thread(() =>
            {
                while (!_runDiscoToken.IsCancellationRequested)
                {
                    SendDiscovery();
                    // We ask for a ping to check that everything is still present every minute
                    Thread.Sleep((int)update.TotalMilliseconds);
                    // Check if we do have devices left for more than 3 updates
                    var expiredDevices = new List<DeviceDetails>();
                    lock (_deviceDetailsLock)
                    {
                        for (int i = _deviceDetails.Count - 1; i >= 0; i--)
                        {
                            if ((DateTimeOffset.UtcNow - _deviceDetails[i].LastUpdate).TotalMilliseconds > update.TotalMilliseconds * 3)
                            {
                                _deviceDetails[i].DeviceStatus = DeviceStatus.Absent;
                                expiredDevices.Add(_deviceDetails[i]);
                                _deviceDetails.RemoveAt(i);
                            }
                        }
                    }

                    foreach (var device in expiredDevices)
                    {
                        OnDeviceLeft?.Invoke(device);
                    }
                }
            });

            _runReceive = new Thread(() =>
            {
                while (!_runReceiveToken.IsCancellationRequested)
                {
                    try
                    {
                        // We want to receive from anyone in the network
                        var from = new IPEndPoint(0, 0);
                        // This is bloking up to the moment something is received but we do only receive very small parts
                        var recvBuffer = _udpClient.Receive(ref from);
                        var res = DiscoveryMessage.DecodeMessage(recvBuffer, out DiscoveryMessageType messageType, out sbyte id, out IPAddress ip, out byte[] payload);

                        Console.WriteLine($"MSG: {BitConverter.ToString(recvBuffer)} message type: {messageType}, valid: {res}");

                        if (!res || messageType == DiscoveryMessageType.Discovery)
                        {
                            continue;
                        }

                        var devDetails = new DeviceDetails();
                        DeviceDetails? oldDevDeatils;

                        devDetails.Id = id;

                        if (messageType == DiscoveryMessageType.Byebye)
                        {
                            lock (_deviceDetailsLock)
                            {
                                oldDevDeatils = _deviceDetails.Where(m => m.Id == devDetails.Id).FirstOrDefault();
                                if (oldDevDeatils != null)
                                {
                                    _deviceDetails.Remove(oldDevDeatils);
                                    oldDevDeatils.DeviceStatus = DeviceStatus.Laaving;
                                    oldDevDeatils.LastUpdate = DateTimeOffset.UtcNow;
                                }
                            }

                            if (oldDevDeatils != null)
                            {
                                OnDeviceLeft?.Invoke(oldDevDeatils);
                            }

                            continue;
                        }

                        devDetails.IPAddress = ip;

                        // Now check how many capabilities
                        if (payload != null && payload.Length > 0)
                        {
                            devDetails.DeviceCapacity = (DeviceCapability)payload[0];                            
                        }

                        DeviceDetails? joinedDevice = null;
                        lock (_deviceDetailsLock)
                        {
                            oldDevDeatils = _deviceDetails.Where(m => m.Id == devDetails.Id).FirstOrDefault();
                            if (oldDevDeatils != null)
                            {
                                oldDevDeatils.LastUpdate = DateTimeOffset.UtcNow;
                                if ((oldDevDeatils.DeviceStatus != DeviceStatus.Joining) || (oldDevDeatils.DeviceCapacity != devDetails.DeviceCapacity))
                                {
                                    oldDevDeatils.DeviceCapacity = devDetails.DeviceCapacity;
                                    oldDevDeatils.DeviceStatus = DeviceStatus.Joining;
                                    joinedDevice = oldDevDeatils;
                                }
                            }
                            else
                            {
                                devDetails.LastUpdate = DateTimeOffset.UtcNow;
                                _deviceDetails.Add(devDetails);
                                joinedDevice = devDetails;
                            }
                        }

                        if (joinedDevice != null)
                        {
                            OnDeviceJoined?.Invoke(joinedDevice);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"UDP Receive: {ex}");
                    }
                }
            });

            _runReceive.Start();
            _runDiscovery.Start();
        }

        /// <summary>
        /// Releases all resources used by the discovery service.
        /// </summary>
        public void Dispose()
        {
            _runDiscoToken?.Cancel();
            _runReceiveToken?.Cancel();
            _udpClient?.Dispose();
            _runDiscovery?.Join();
            _runReceive?.Join();
        }

        /// <summary>
        /// Sends a discovery broadcast message to find devices on the network.
        /// </summary>
        public void SendDiscovery()
        {
            try
            {
                var data = DiscoveryMessage.CreateMessage(DiscoveryMessageType.Discovery, 0, null!, null!);
                _udpClient.Send(data, data.Length, "255.255.255.255", BindingPort);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{nameof(SendDiscovery)}: {ex}");
            }
        }
    }
}
