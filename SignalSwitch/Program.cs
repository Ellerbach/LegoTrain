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
using nanoDiscovery.Common;

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
        private static Blinky _blinky;
        private  static int _tries = 0;

        public static void Main()
        {
            Debug.WriteLine("Hello Lego Signal and Switch");

            // Try to read the configuration
            _appConfiguration = AppConfiguration.Load();
            if (AppConfiguration == null)
            {
                _appConfiguration = new AppConfiguration();
                _appConfiguration.LedGpio = 8;
                _appConfiguration.SwitchActivated = false;
                _appConfiguration.SwitchActivated = false;
                _appConfiguration.ServoGpio = 7;
                _appConfiguration.SignalGpioGreen = 9;
                _appConfiguration.SignalGpioRed = 10;
                _appConfiguration.ServoMinimumPulse = 1000;
                _appConfiguration.ServoMaximumPulse = 2100;
                _appConfiguration.Save();
            }

            ConfigurationController.AppConfiguration = _appConfiguration;
            SetSignal();
            SetSwitch();

            _wifiApMode = Wireless80211.ConnectOrSetAp();
            _blinky = new Blinky(_appConfiguration.LedGpio);

            // If we are in normal mode, advertize the service
            if (!_wifiApMode)
            {
                SetDiscovery();
                _blinky.BlinkNormal();
            }
            else
            {
                _blinky.BlinkWaiWifi();
            }

            Debug.WriteLine($"Connected with wifi credentials. IP Address: {(_wifiApMode ? WirelessAP.GetIP() : Wireless80211.GetCurrentIPAddress())}");
            _server = new WebServer(80, HttpProtocol.Http, new Type[] { typeof(ApiController), typeof(ConfigurationController) });
            // Add a handler for commands that are received by the server.
            _server.CommandReceived += ServerCommandReceived;
            _server.WebServerStatusChanged += WebServerStatusChanged;

            // Start the server.
            _server.Start();

            AppConfiguration.OnConfigurationUpdated += OnConfigurationUpdated;

            Thread.Sleep(Timeout.Infinite);
        }

        private static void WebServerStatusChanged(object obj, WebServerStatusEventArgs e)
        {
            if (e.Status == WebServerStatus.Stopped)
            {
                if (_tries++ < 5)
                {
                    _server.Start();
                }
                else
                {
                    Sleep.EnableWakeupByTimer(new TimeSpan(0, 0, 0, 1));
                    Sleep.StartDeepSleep();
                }
            }
        }

        private static void OnConfigurationUpdated(object sender, ConfigurationEventArgs e)
        {
            Debug.WriteLine($"Parameter updated: {e.ParamName}");
            // Check the mode
            if (e.ParamName.StartsWith("Signal"))
            {
                SetSignal();
            }
            else if (e.ParamName.StartsWith("Servo") || e.ParamName.StartsWith("Switch"))
            {
                SetSwitch();
            }
            
            if (e.ParamName.StartsWith("Device") || e.ParamName.EndsWith("Activated"))
            {
                SetDiscovery();
            }
            else
            {
                _blinky?.Dispose();
                _blinky.BlinkNormal();
            }
        }

        private static void SetSwitch()
        {
            if (_appConfiguration.SwitchActivated)
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
                    Debug.WriteLine($"Invalid Switch configuration: {ex.Message}");
                }
            }
        }

        private static void SetSignal()
        {
            if (_appConfiguration.SignalActivated)
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
                    Console.WriteLine($"Invalid Signal configuration: {ex.Message}");
                }
            }
        }

        private static void SetDiscovery()
        {
            Debug.WriteLine("Starting discovery service");
            // We strart the discovery service

            DeviceCapability capacities = DeviceCapability.None;
            if (_appConfiguration.SignalActivated)
            {
                capacities |= DeviceCapability.Signal;
            }

            if (_appConfiguration.SwitchActivated)
            {
                capacities |= DeviceCapability.Switch;
            }

            _legoDiscoToken?.Cancel();
            _legoDiscovery?.Dispose();
            _legoDiscovery = new LegoDiscovery(IPAddress.Parse(Wireless80211.GetCurrentIPAddress()), _appConfiguration.DeviceId, capacities);
            _legoDiscoToken?.Dispose();
            _legoDiscoToken = new CancellationTokenSource();
            _legoDiscovery.SendCapabilities(IPAddress.Parse("255.255.255.255"));
            _legoDiscovery.Run(_legoDiscoToken.Token);
        }

        private static void ServerCommandReceived(object obj, WebServerEventArgs e)
        {
            // Not enough memory to handle those!
            if (e.Context.Request.RawUrl.StartsWith("/style.css"))
            {
                e.Context.Response.ContentType = "text/css";
                WebServer.OutPutStream(e.Context.Response, ResourceWeb.GetString(ResourceWeb.StringResources.style));
                return;
            }
            //else if (e.Context.Request.RawUrl.StartsWith("/favicon.ico"))
            //{
            //    var ico = ResourceWeb.GetBytes(ResourceWeb.BinaryResources.favicon);
            //    e.Context.Response.ContentType = "image/ico";
            //    e.Context.Response.ContentLength64 = ico.Length;
            //    e.Context.Response.OutputStream.Write(ico, 0, ico.Length);
            //    return;
            //}

            if (_wifiApMode)
            {
                WebServerCommon.SetupWifi(e);
            }
            else
            {
                string toOutput = "<html><head><title>Lego Signal/Switch</title><link rel=\"stylesheet\" href=\"style.css\"></head><body>";
                if (AppConfiguration.SignalActivated)
                {
                    toOutput += $"Your Signal configuration is <b>{(Signal == null ? "incorrect" : "valid")}</b>.<br/>";
                }

                if (AppConfiguration.SwitchActivated)
                {
                    toOutput += $"Your Switch configuration is <b>{(Switch == null ? "incorrect" : "valid")}</b>.<br/>";
                }

                if (!AppConfiguration.SignalActivated && !AppConfiguration.SwitchActivated)
                {
                    toOutput += "You haven't set a proper device type. Go to <a href=\"/config\">configuration</a><br/>";
                }
                else
                {
                    toOutput += $"Your device is properly set as {(AppConfiguration.SignalActivated && AppConfiguration.SwitchActivated ? "both Signal and Switch.": (AppConfiguration.SignalActivated ? "signal." : "swith"))}<br/>";
                    toOutput += $"Your device ID is {(AppConfiguration.DeviceId < 0 ? "invalid, it must be more or equal to 1." : AppConfiguration.DeviceId)}.<br>";
                }

                toOutput += "To configure your device please go to <a href=\"/config\">configuration</a>.<br/>";
                toOutput += "Reset your wifi by cliking <a href=\"/resetwifi\">here</a>.<br>";
                toOutput += "</body></html>";
                WebServer.OutPutStream(e.Context.Response, toOutput);
                return;
            }
        }
    }
}
