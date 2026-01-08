// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using System.Net;
using System;
using System.Threading;
using System.Net.Sockets;
using System.Diagnostics;
using nanoDiscovery;
using nanoDiscovery.Common;

namespace SharedServices.Services
{
    /// <summary>
    /// Manages device discovery for nanoFramework devices on the Lego train network.
    /// </summary>
    public class LegoDiscovery : IDisposable
    {
        private const int BindingPort = 2024;
        private UdpClient _udpClient;
        private IPAddress _ipAddress;
        private DeviceCapability _capabilities;
        private int _deviceId;
        private CancellationTokenSource _tokenSource;
        private Thread _runner;

        /// <summary>
        /// Gets the IP address of the server which sent a discovery request.
        /// </summary>
        public IPAddress ServerAddress { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LegoDiscovery"/> class.
        /// </summary>
        /// <param name="ipaddess">The IP address of this device.</param>
        /// <param name="deviceId">The unique identifier for this device.</param>
        /// <param name="capabilities">The capabilities of this device.</param>
        public LegoDiscovery(IPAddress ipaddess, int deviceId, DeviceCapability capabilities)
        {
            _udpClient = new UdpClient();
            _ipAddress = ipaddess;
            _capabilities = capabilities;
            _deviceId = deviceId;
            IsRunning = false;

            // Bind the UDP Client on the port on any address
            _udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, BindingPort));
        }

        /// <summary>
        /// Releases all resources used by the discovery service.
        /// </summary>
        public void Dispose()
        {
            SendByeBye();

            Stop();
            // Wait a bit
            if (_runner != null)
            {
                _runner.Join(1500);
            }

            _udpClient?.Dispose();
        }

        /// <summary>
        /// Starts listening for discovery messages.
        /// </summary>
        /// <param name="token">The cancellation token for stopping the service.</param>
        public void Run(CancellationToken token)
        {
            IsRunning = true;
            _tokenSource = new CancellationTokenSource();
            // Allow to receive answers from anyone on the network
            var from = (EndPoint)(new IPEndPoint(0, 0));
            _runner = new Thread(() =>
            {
                while (!token.IsCancellationRequested && !_tokenSource.IsCancellationRequested)
                {
                    try
                    {
                        if (_udpClient.Available > 0)
                        {
                            byte[] recvBuffer = new byte[_udpClient.Available];
                            _udpClient.Client.ReceiveFrom(recvBuffer, ref from);
                            var res = DiscoveryMessage.DecodeMessage(recvBuffer, out DiscoveryMessageType messageType, out sbyte id, out IPAddress ip, out byte[] payload);

                            Console.WriteLine($"MSG: {BitConverter.ToString(recvBuffer)}, decode: {res}, type: {messageType}");
                            if (res && messageType == DiscoveryMessageType.Discovery)
                            {
                                ServerAddress = ((IPEndPoint)from).Address;
                                SendCapabilities(((IPEndPoint)from).Address);
                            }
                        }

                        // We do answer in about 1 second, no need to put stress on this
                        _tokenSource.Token.WaitHandle.WaitOne(1000, true);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"LegoDiscovery: {ex.Message}");
                    }
                }

                IsRunning = false;
            });
            _runner.Start();
        }

        /// <summary>
        /// Gets a value indicating whether the discovery service is currently running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Stops the discovery service.
        /// </summary>
        public void Stop() => _tokenSource?.Cancel();

        /// <summary>
        /// Sends a capabilities message to the specified IP address.
        /// </summary>
        /// <param name="ip">The IP address to send the capabilities to.</param>
        public void SendCapabilities(IPAddress ip)
        {
            try
            {
                var payload = new byte[] { (byte)_capabilities };
                var data = DiscoveryMessage.CreateMessage(DiscoveryMessageType.Capabilities, (sbyte)_deviceId, _ipAddress, payload);

                _udpClient.Send(data, 0, data.Length, new IPEndPoint(ip, BindingPort));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Dico capacity: {ex.Message}");
            }
        }

        /// <summary>
        /// Sends a goodbye message to notify the network that this device is leaving.
        /// </summary>
        public void SendByeBye()
        {
            try
            {
                var data = DiscoveryMessage.CreateMessage(DiscoveryMessageType.Byebye, (sbyte)_deviceId, _ipAddress, null);
                _udpClient.Send(data, 0, data.Length, new IPEndPoint(IPAddress.Parse("255.255.255.255"), BindingPort));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Dico bybye: {ex.Message}");
            }
        }
    }
}
