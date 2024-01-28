// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using LegoTrain.Models.Device;
using nanoDiscovery;
using nanoDiscovery.Common;
using System.Net;
using System.Net.Sockets;

namespace LegoTrain.Services
{
    public class LegoDiscovery : IDisposable
    {
        private const int BindingPort = 2024;
        private UdpClient _udpClient;
        private Thread _runDiscovery;
        private CancellationTokenSource _runDiscoToken;
        private Thread _runReceive;
        private CancellationTokenSource _runReceiveToken;
        private List<DeviceDetails> _deviceDetails = new List<DeviceDetails>();

        public List<DeviceDetails> DeviceDetails => _deviceDetails;

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
                    for (int i =0; i< _deviceDetails.Count; i++)
                    {
                        if ((DateTimeOffset.UtcNow - _deviceDetails[i].LastUpdate).TotalMilliseconds > update.TotalMilliseconds * 3)
                        {
                            _deviceDetails[i].DeviceStatus = DeviceStatus.Absent;
                            // TODO: Send event

                            _deviceDetails.Remove(_deviceDetails[i]);
                        }
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
                        oldDevDeatils = _deviceDetails.Where(m => m.Id == devDetails.Id).FirstOrDefault();

                        if (messageType == DiscoveryMessageType.Byebye)
                        {
                            if (oldDevDeatils != null)
                            {
                                _deviceDetails.Remove(oldDevDeatils);
                                // TODO: notify with event
                                oldDevDeatils.DeviceStatus = DeviceStatus.Laaving;
                                oldDevDeatils.LastUpdate = DateTimeOffset.UtcNow;
                            }

                            continue;
                        }

                        devDetails.IPAddress = ip;

                        // Now check how many capabilities
                        if (payload != null && payload.Length > 0)
                        {
                            devDetails.DeviceCapacity = (DeviceCapability)payload[0];                            
                        }

                        // Check if we already have one
                        if (oldDevDeatils != null)
                        {
                            oldDevDeatils.LastUpdate = DateTimeOffset.UtcNow;
                            // Check if status is different than Joining
                            if ((oldDevDeatils.DeviceStatus != DeviceStatus.Joining) || (oldDevDeatils.DeviceCapacity != devDetails.DeviceCapacity))
                            {
                                oldDevDeatils.DeviceCapacity = devDetails.DeviceCapacity;
                                oldDevDeatils.DeviceStatus = DeviceStatus.Joining;
                                // TODO Send notification
                            }
                        }
                        else
                        {
                            // TODO Send notification
                            devDetails.LastUpdate = DateTimeOffset.UtcNow;
                            _deviceDetails.Add(devDetails);
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

        public void Dispose()
        {
            _runDiscoToken?.Cancel();
            _runReceiveToken?.Cancel();
            _udpClient?.Dispose();
            _runDiscovery?.Join();
            _runReceive?.Join();
        }

        public void SendDiscovery()
        {
            try
            {
                var data = DiscoveryMessage.CreateMessage(DiscoveryMessageType.Discovery, 0, null, null);
                _udpClient.Send(data, data.Length, "255.255.255.255", BindingPort);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{nameof(SendDiscovery)}: {ex}");
            }
        }
    }
}
