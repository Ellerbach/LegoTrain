// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using nanoFramework.Networking;
using System;
using System.Device.Wifi;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;

namespace SharedServices.Services
{
    /// <summary>
    /// Provides wireless 802.11 (WiFi) configuration and management for nanoFramework devices.
    /// </summary>
    class Wireless80211
    {
        /// <summary>
        /// Checks if wireless configuration is enabled.
        /// </summary>
        /// <returns>True if wireless is configured with an SSID, false otherwise.</returns>
        public static bool IsEnabled()
        {
            Wireless80211Configuration wconf = GetConfiguration();
            return !string.IsNullOrEmpty(wconf.Ssid);
        }

        /// <summary>
        /// Gets the current IP address. Only valid if successfully provisioned and connected.
        /// </summary>
        /// <returns>IP address string.</returns>
        public static string GetCurrentIPAddress()
        {
            NetworkInterface ni = NetworkInterface.GetAllNetworkInterfaces()[0];

            // get first NI ( Wifi on ESP32 )
            return ni.IPv4Address.ToString();
        }

        /// <summary>
        /// Connects to the WiFi network or sets up the Access Point mode if connection fails.
        /// </summary>
        /// <returns>True if access point is setup, false if connected to WiFi.</returns>
        public static bool ConnectOrSetAp()
        {
            if (IsEnabled())
            {
                Debug.WriteLine("Wireless client activated");
                if (!WifiNetworkHelper.Reconnect(true, token: new CancellationTokenSource(10_000).Token))
                {
                    WirelessAP.SetWifiAp();
                    return true;
                }
            }
            else
            {
                WirelessAP.SetWifiAp();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Disables the wireless station interface.
        /// </summary>
        public static void Disable()
        {
            Wireless80211Configuration wconf = GetConfiguration();
            wconf.Options = Wireless80211Configuration.ConfigurationOptions.None | Wireless80211Configuration.ConfigurationOptions.SmartConfig;
            wconf.SaveConfiguration();
        }

        /// <summary>
        /// Configures and enables the wireless station interface.
        /// </summary>
        /// <param name="ssid">The SSID of the network to connect to.</param>
        /// <param name="password">The password for the network.</param>
        /// <returns>True if connection is successful, false otherwise.</returns>
        public static bool Configure(string ssid, string password)
        {
            // Make sure we are disconnected before we start connecting otherwise
            // ConnectDhcp will just return success instead of reconnecting.
            WifiAdapter wa = WifiAdapter.FindAllAdapters()[0];
            wa.Disconnect();

            CancellationTokenSource cs = new(30_000);
            Console.WriteLine("ConnectDHCP");
            WifiNetworkHelper.Disconnect();

            // Reconfigure properly the normal wifi
            Wireless80211Configuration wconf = GetConfiguration();
            wconf.Options = Wireless80211Configuration.ConfigurationOptions.AutoConnect | Wireless80211Configuration.ConfigurationOptions.Enable;
            wconf.Ssid = ssid;
            wconf.Password = password;
            wconf.SaveConfiguration();

            WifiNetworkHelper.Disconnect();
            bool success;

            success = WifiNetworkHelper.ConnectDhcp(ssid, password, WifiReconnectionKind.Automatic, true, token: cs.Token);

            if (!success)
            {
                wa.Disconnect();
                // Bug in network helper, we've most likely try to connect before, let's make it manual
                var res = wa.Connect(ssid, WifiReconnectionKind.Automatic, password);
                success = res.ConnectionStatus == WifiConnectionStatus.Success;
                Console.WriteLine($"Connected: {res.ConnectionStatus}");
            }            

            Console.WriteLine($"ConnectDHCP exit {success}");
            return success;
        }

        /// <summary>
        /// Gets the wireless station configuration.
        /// </summary>
        /// <returns>The Wireless80211Configuration object.</returns>
        public static Wireless80211Configuration GetConfiguration()
        {
            NetworkInterface ni = GetInterface();
            return Wireless80211Configuration.GetAllWireless80211Configurations()[ni.SpecificConfigId];
        }

        /// <summary>
        /// Gets the wireless network interface.
        /// </summary>
        /// <returns>The NetworkInterface for wireless, or null if not found.</returns>
        public static NetworkInterface GetInterface()
        {
            NetworkInterface[] Interfaces = NetworkInterface.GetAllNetworkInterfaces();

            // Find WirelessAP interface
            foreach (NetworkInterface ni in Interfaces)
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    return ni;
                }
            }
            return null;
        }
    }
}
