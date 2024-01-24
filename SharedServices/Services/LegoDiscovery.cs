// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using System.Net;
using System.Text;
using System;
using System.Threading;
using System.Net.Sockets;
using System.Diagnostics;
using nanoDiscovery;

namespace SharedServices.Services
{
    public class LegoDiscovery : IDisposable
    {
        public const string Signal = ":SI";
        public const string Switch = ":SW";
        public const string Both = ":SW:SI";
        public const string Infrared = ":IR";

        private const int BindingPort = 2024;
        private UdpClient _udpClient;
        private IPAddress _ipAddress;
        private string _capabilities;
        private int _deviceId;
        private CancellationTokenSource _tokenSource;
        private Thread _runner;

        public LegoDiscovery(IPAddress ipaddess, int deviceId, string capabilities)
        {
            _udpClient = new UdpClient();
            _ipAddress = ipaddess;
            _capabilities = capabilities;
            _deviceId = deviceId;
            IsRunning = false;

            // Bind the UDP Client on the port on any address
            _udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, BindingPort));
        }

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

        public bool IsRunning { get; private set; }

        public void Stop() => _tokenSource?.Cancel();

        public void SendCapabilities(IPAddress ip = default)
        {
            try
            {
                string capabilities = $"{_capabilities.TrimStart(':')}";
                var payload = Encoding.UTF8.GetBytes(capabilities);
                var data = DiscoveryMessage.CreateMessage(DiscoveryMessageType.Capabilities, (sbyte)_deviceId, _ipAddress, payload);
                if(ip == default)
                {
                    ip = IPAddress.Parse("255.255.255.255");
                }

                _udpClient.Send(data, 0, data.Length, new IPEndPoint(ip, BindingPort));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Dico capacity: {ex.Message}");
            }
        }

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
