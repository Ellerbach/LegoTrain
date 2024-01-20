// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using nanoFramework.Hardware.Esp32;
using nanoFramework.WebServer;
using SharedServices.Models;
using LegoElement.Controllers;
using LegoElement.Models;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using SharedServices.Controllers;
using SharedServices.Services;
using SignalSwitch;

namespace LegoElement
{
    public class Application
    {
        public static AppConfiguration AppConfiguration { get => _appConfiguration; }
        public static Signal Signal { get => _signal; }
        public static Switch Switch { get => _switch; }

        private static AppConfiguration _appConfiguration;
        private static WebServer _server;
        private static Signal _signal;
        private static Switch _switch;
        private static LegoDiscovery _legoDiscovery;
        private static CancellationTokenSource _legoDiscoToken;
        private static bool _wifiApMode = false;

        public static void Main()
        {
            Debug.WriteLine("Hello Lego Signal and Switch");

            // Try to read the configuration
            _appConfiguration = AppConfiguration.Load();
            if (AppConfiguration != null)
            {
                SetSignal();
                SetSwitch();
            }

            _appConfiguration = AppConfiguration ?? new AppConfiguration();
            ConfigurationController.AppConfiguration = _appConfiguration;

            _wifiApMode = Wireless80211.ConnectOrSetAp();

            // If we are in normal mode, advertize the service
            if (!_wifiApMode)
            {
                SetDiscovery();
            }

            Console.WriteLine($"Connected with wifi credentials. IP Address: {(_wifiApMode ? WirelessAP.GetIP() : Wireless80211.GetCurrentIPAddress())}");
            _server = new WebServer(80, HttpProtocol.Http, new Type[] { typeof(ApiController), typeof(ConfigurationController) });
            // Add a handler for commands that are received by the server.
            _server.CommandReceived += ServerCommandReceived;

            // Start the server.
            _server.Start();

            AppConfiguration.OnConfigurationUpdated += OnConfigurationUpdated;

            Thread.Sleep(Timeout.Infinite);
        }

        private static void OnConfigurationUpdated(object sender, ConfigurationEventArgs e)
        {
            Debug.WriteLine($"Parameter updated: {e.ParamName}");
            // Check the mode
            if (e.ParamName.StartsWith("Signal"))
            {
                SetSignal();
            }
            else if (e.ParamName.StartsWith("Servo"))
            {
                SetSwitch();
            }
            else if(e.ParamName.StartsWith("Device"))
            {
                SetDiscovery();
            }
        }

        private static void SetSwitch()
        {
            if (AppConfiguration.DeviceType == AppConfiguration.Switch || AppConfiguration.DeviceType == AppConfiguration.Both)
            {
                try
                {
                    if (_switch != null)
                    {
                        _switch.Dispose();
                    }

                    if (AppConfiguration.ServoGpio >= 0)
                    {
                        Configuration.SetPinFunction(AppConfiguration.ServoGpio, DeviceFunction.PWM1);
                    }

                    _switch = new Switch(AppConfiguration.ServoGpio, AppConfiguration.ServoMinimumPulse, AppConfiguration.ServoMaximumPulse);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Invalid LegoInfrared configuration: {ex.Message}");
                }
            }
        }

        private static void SetSignal()
        {
            if (AppConfiguration.DeviceType == AppConfiguration.Signal || AppConfiguration.DeviceType == AppConfiguration.Both)
            {
                try
                {
                    if (_signal != null)
                    {
                        _signal.Dispose();
                    }

                    _signal = new Signal(AppConfiguration.SignalGpioRed, AppConfiguration.SignalGpioGreen);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Invalid LegoInfrared configuration: {ex.Message}");
                }
            }
        }

        private static void SetDiscovery()
        {
            Debug.WriteLine("Starting discovery service");
            // We strart the discovery service
            string capacities = string.Empty;
            switch (_appConfiguration.DeviceType)
            {
                case AppConfiguration.Both:
                    capacities = LegoDiscovery.Both;
                    break;
                case AppConfiguration.Signal:
                    capacities = LegoDiscovery.Signal;
                    break;
                case AppConfiguration.Switch:
                    capacities = LegoDiscovery.Switch;
                    break;
                default:
                    break;
            }

            _legoDiscoToken?.Cancel();
            _legoDiscovery?.Dispose();
            _legoDiscovery = new LegoDiscovery(IPAddress.Parse(Wireless80211.GetCurrentIPAddress()), _appConfiguration.DeviceId, capacities);
            _legoDiscoToken?.Dispose();
            _legoDiscoToken = new CancellationTokenSource();
            _legoDiscovery.SendCapabilities();
            _legoDiscovery.Run(_legoDiscoToken.Token);
        }

        private static void ServerCommandReceived(object obj, WebServerEventArgs e)
        {
            if (e.Context.Request.RawUrl.StartsWith("/style.css"))
            {
                e.Context.Response.ContentType = "text/css";
                WebServer.OutPutStream(e.Context.Response, ResourceWeb.GetString(ResourceWeb.StringResources.style));
                return;
            }
            else if (e.Context.Request.RawUrl.StartsWith("/favicon.ico"))
            {
                var ico = ResourceWeb.GetBytes(ResourceWeb.BinaryResources.favicon);
                e.Context.Response.ContentType = "image/ico";
                e.Context.Response.ContentLength64 = ico.Length;
                e.Context.Response.OutputStream.Write(ico, 0, ico.Length);
                return;
            }

            if (_wifiApMode)
            {
                WebServerCommon.SetupWifi(e);
            }
            else
            {
                string toOutput = "<html><head><title>Lego Signal/Switch</title></head><body>";
                if (AppConfiguration.DeviceType == AppConfiguration.Signal || AppConfiguration.DeviceType == AppConfiguration.Both)
                {
                    toOutput += $"Your Signal configuration is <b>{(Signal == null ? "incorrect" : "valid")}</b>.<br/>";
                }

                if (AppConfiguration.DeviceType == AppConfiguration.Switch || AppConfiguration.DeviceType == AppConfiguration.Both)
                {
                    toOutput += $"Your Switch configuration is <b>{(Switch == null ? "incorrect" : "valid")}</b>.<br/>";
                }

                if ((AppConfiguration.DeviceType != AppConfiguration.Signal) &&
                    (AppConfiguration.DeviceType != AppConfiguration.Both) &&
                    (AppConfiguration.DeviceType != AppConfiguration.Switch))
                {
                    toOutput += "You haven't set a proper device type. Go to <a href=\"/configuration/config\">configuration</a><br/>";
                }
                else
                {
                    toOutput += $"Your device is properly set as {(AppConfiguration.DeviceType != AppConfiguration.Both ? AppConfiguration.DeviceType : "both Signal and Switch.<br/>")}";
                    toOutput += $"Your device ID is {(AppConfiguration.DeviceId < 0 ? "invalid, it must be positive or zero." : AppConfiguration.DeviceId)}.<br>";
                }

                toOutput += "To configure your device please go to <a href=\"/configuration/config\">configuration</a>.<br/>";
                toOutput += "Reset your wifi by cliking <a href=\"/configuration/resetwifi\">here</a>.";
                toOutput += "</body></html>";
                WebServer.OutPutStream(e.Context.Response, toOutput);
                return;
            }
        }
    }
}
